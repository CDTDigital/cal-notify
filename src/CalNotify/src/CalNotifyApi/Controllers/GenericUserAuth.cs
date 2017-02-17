using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CalNotify.Events;
using CalNotify.Models.Addresses;
using CalNotify.Models.Auth;
using CalNotify.Models.Responses;
using CalNotify.Models.Services;
using CalNotify.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotify.Controllers.GenericUsers
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


        // Tokens 
        private readonly TokenService _tokenService;



        public GenericUserAuth(
            BusinessDbContext context,
            ValidationSender validation,
            ILogger<GenericUserAuth> logger,
            ITokenMemoryCache memoryCache,

              TokenService tokenService,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _validation = validation;
            _logger = logger;


            _tokenService = tokenService;
            _memoryCache = memoryCache;

            _hostingEnvironment = hostingEnvironment;
        }




        /// <summary>
        /// Starts  the flow of creating a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost("create"), Consumes("application/json"), Produces("application/json", Type = typeof(ResponseShell<SimpleSuccess>))]
        [SwaggerOperation(operationId: "CreateUser", Tags = new[] { Constants.GenericUserEndpoint })]

        public async Task<IActionResult> CreateWithChallenge([FromBody] TempUser model)
        {


            // Check out our users, if we already someone, then no need to validate, its just an error
            var check = await _context.Users.AnyAsync(user => user.PhoneNumber == model.PhoneNumber || user.Email == model.Email);
            if (check)
            {
                return ResponseShell.Error("GenericUser already has an account");
            }
          

            // TODO: Condtion on us being in our testing environment

            if (Constants.Testing.CheckIfOverride(model) && (_hostingEnvironment.IsDevelopment() || _hostingEnvironment.IsEnvironment("Testing")))
            {
                model.Token = Constants.Testing.TestValidationToken;
                // Hold our token and model for a while to give our user a chance to validate their info
                _memoryCache.SetForChallenge(model);

                // All good thus far, now we just wait on our user to validate
                return ResponseShell.Ok();
            }

            // We prefer to send validation through email but will send through sms if needed
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                await _validation.SendValidationToEmail(model);
            }
            else if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                await _validation.SendValidationToSms(model);
            }
            _memoryCache.SetForChallenge(model);
            return ResponseShell.Ok();


        }

        /// <summary>
        /// Allows setting the user password
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("password"), ProducesResponseType(typeof(ResponseShell<SimpleSuccess>),200)]
        [SwaggerOperation("Set a user passowrd", Tags = new[] { Constants.GenericUserEndpoint })]
        [GenericUserResources.ValidateGenricExistsAttribute]
        public IActionResult SetPassword([FromQuery] string id, [FromQuery, Required,MinLength(6), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")] string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == new Guid(id));
            user.SetPassword(password);
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
        /// <param name="password">Password to use for subsequent account logins.</param>    

        [HttpGet("validate"), ProducesResponseType(typeof(ResponseShell<TokenInfo>), 200)]
        [SwaggerOperation("ValidateToken", Tags = new[] { Constants.GenericUserEndpoint })]
        public async Task<IActionResult> Validate([FromQuery] string token)
        {
            // Get our Saved User from Memory
            var savedUser = new CheckValidationTokenEvent().Process(_memoryCache, token);
            // Create our GenericUser if need be
            var createdGenericUser = new CreateUserEvent().Process(_context, savedUser);
            // Get our token
            var endToken = await _tokenService.GetToken(createdGenericUser);

            _context.SaveChanges();
            // All good, lets give out our token
            return ResponseShell.Ok(endToken);

        }

       


    }
}
