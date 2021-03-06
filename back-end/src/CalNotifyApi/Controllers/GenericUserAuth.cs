﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using CalNotifyApi.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Controllers
{
    [AllowAnonymous]
    [Route(Constants.V1Prefix + "/" + Constants.GenericUserEndpoint)]
    public class GenericUserAuth : Controller
    {
        // DB 
        private readonly BusinessDbContext _context;



        // short lived persistance, for holding validation tokens
        private readonly ITokenMemoryCache _memoryCache;
        private readonly ILogger<GenericUserAuth> _logger;

        private readonly ValidationSender _validation;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ExternalServicesConfig _config;

        // Tokens 
        private readonly TokenService _tokenService;



        public GenericUserAuth(
            BusinessDbContext context,
            ValidationSender validation,
            ILogger<GenericUserAuth> logger,
            ITokenMemoryCache memoryCache,

              TokenService tokenService,
            IHostingEnvironment hostingEnvironment, ExternalServicesConfig config)
        {
            _context = context;
            _validation = validation;
            _logger = logger;


            _tokenService = tokenService;
            _memoryCache = memoryCache;

            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }


      

        /// <summary>
        /// Allows an user to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login"), Consumes("application/json")]
        [SwaggerOperation("USER_LOGIN", Tags = new[] { Constants.GenericUserEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<string>), 200)]
        public virtual async Task<IActionResult> UserLogin([FromBody] UserLoginEvent model)
        {
            // The eventual user

            var validatedUser = _context.AllUsers.Where(u => u.Enabled).FirstOrDefault(x => x.Email == model.ContactInfo || x.PhoneNumber == model.ContactInfo.CleanPhone());
            if (validatedUser == null)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error("Could not find an account with that information");
            }
            var passCheck = validatedUser.VerifyPassowrd(model.Password);
            if (passCheck != PasswordVerificationResult.Success)
            {
                return ResponseShell.Error("Could not find an account with that information");
            }

            // Get our token
            var token = await _tokenService.GetToken(validatedUser);

            // overloading the login for webadmins too
            if (validatedUser is WebAdmin)
            {
                return ResponseShell.Ok($"{_config.Urls.Frontend}/{_config.Pages.AdminPage}?user={token.UserId}&token={token.Token}");
            }

            return ResponseShell.Ok($"{_config.Urls.Frontend}/{_config.Pages.AccountPage}?user={token.UserId}&token={token.Token}");
        }


        /// <summary>
        /// Starts  the flow of creating a new user.
        /// </summary>
        /// <remarks>
        ///  From here we are collecting the prerequiste information which a user needs inorder to utilize the notification service.
        /// Internal this all begins with the creation of a temporary user with the provided information. Thus user, needs to be validated through 
        /// one of the provided contact methods, either email or sms. Depending on the provided information we send out a verification/confirmation message
        /// asking the user to verify they are the account holder's of the email or sms number provided.
        /// system
        /// </remarks>
        /// <returns></returns>
        [HttpPost("create"), Consumes("application/json"), Produces("application/json", Type = typeof(ResponseShell<SimpleSuccess>))]
        [SwaggerOperation(operationId: "CreateUser", Tags = new[] { Constants.GenericUserEndpoint })]
        public async Task<IActionResult> CreateWithChallenge([FromBody] TempUser tempUser)
        {
            await new CreateDisabledUserAccount().Process(_hostingEnvironment, _memoryCache, _context, _validation, tempUser);
            return ResponseShell.Ok();
        }


        /// <summary>
        /// Validates the  token provided in the earlier create call, and returns the newly created user
        /// </summary>
        /// <remarks>
        /// If the code entered is correct then a new GenericUser will be created.
        /// We are validating the token and input, and with a successful validation we go ahead and create a new GenericUser, and setup other infrastructure to support them, such as a stripe GenericUser, etc..
        /// Once you get back a token, [see an example of using this token to authorize swagger](token_example.gif).
        /// </remarks>
        /// <param name="token">Token which was sent to verify user owns messaing account.</param>
        [HttpGet("validate"), ProducesResponseType(typeof(ResponseShell<TokenInfo>), 200)]
        [SwaggerOperation("ValidateToken", Tags = new[] { Constants.GenericUserEndpoint })]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            TempUser savedUser;
            try
            {
                // Get our Saved User from Memory
                savedUser = new CheckValidationTokenEvent().Process(_memoryCache, token);
            }
            catch (CheckValidationTokenException e)
            {
                return Redirect($"{_config.Urls.Frontend}/{_config.Pages.ResendPage}");
            }
            savedUser.PhoneNumber = savedUser.PhoneNumber.CleanPhone();
            var exisitingUser = _context.AllUsers.FirstOrDefault(x => x.Id == savedUser.Id);

            // we  have a user at this point, otherwise we would have thrown our processer error earlier

            // A new user, validate the first avaible sms
            if (!exisitingUser.Enabled)
            {
                // we call in order the Enable and Create events, which handle the prerequiste logic for handling either case
                exisitingUser = new EnableUserEvent().Process(_context, token, exisitingUser, savedUser);
            }
            else
            {
                // for exisiting users we dont need to create them but just to possibly validate a new communication channel
                exisitingUser = new ValidateExistingUserCommunication().Process(_context, savedUser);

            }
            var endToken = await _tokenService.GetToken(exisitingUser);
            return Redirect($"{_config.Urls.Frontend}/{_config.Pages.AccountPage}?user={endToken.UserId}&token={endToken.Token}");


        }





    }
}
