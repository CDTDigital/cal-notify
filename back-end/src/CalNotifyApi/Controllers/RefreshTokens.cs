using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using CalNotifyApi.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Controllers
{
    [Route(Constants.V1Prefix + "/" + Constants.TokenEndpoint)]
    public class RefreshTokens : Controller
    {
        // DB 
        private readonly BusinessDbContext _context;
        private readonly ISmsSender _smsSender;

        // short lived persistance, for holding validation tokens
        private readonly ITokenMemoryCache _memoryCache;
        private readonly ILogger<RefreshTokens> _logger;

        private readonly TokenService _tokenService;

        private readonly IHostingEnvironment _hostingEnvironment;

        public RefreshTokens(BusinessDbContext context, ISmsSender smsSender,
              ILogger<RefreshTokens> logger,
            ITokenMemoryCache memoryCache,
             TokenAuthOptions tokenOptions,
              TokenService tokenService, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _smsSender = smsSender;
            _logger = logger;
            _memoryCache = memoryCache;

            _tokenService = tokenService;
            _hostingEnvironment = hostingEnvironment;
        }


        /// <summary>
        /// Provides a endpoint for client side applications to refresh a users token via a sms sent token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("refresh"), Consumes("application/json"), Produces("application/json", Type = typeof(ResponseShell<SimpleSuccess>))]
        [SwaggerOperation(operationId: "RefreshTokens", Tags = new[] { Constants.AuthorizationTag })]
        public async Task<IActionResult> Refresh([FromBody] RefreshTempUser model)
        {
 
            var existing = _context.Users.FirstOrDefault(user => user.PhoneNumber == model.PhoneNumber.CleanPhone());
            if (existing == null)
            {
                return ResponseShell.Error("Could not find user.", new List<string>()
                {
                    "Either the GenericUser account has not been created",
                    "Or service is temporarily unavailable"
                });
            }

            if (Constants.Testing.CheckIfOverride(existing) && (_hostingEnvironment.IsDevelopment() || _hostingEnvironment.IsEnvironment("Testing")))
            {
                existing.Token = Constants.Testing.TestValidationToken;
                // Hold our token and model for a while to give our user a chance to validate their info
                _memoryCache.SetForChallenge(new TempUser(existing));

                // All good thus far, now we just wait on our user to validate
                return ResponseShell.Ok(new SimpleSuccess() { Success = true });
            }


            // Fire off our validation
            var token = await _smsSender.SendValidationToSms(new TempUser()
            {
                Email = existing.Email,
                Id = existing.Id,
            });

            existing.Token = token;
            // Hold our token and model for a while to give our user a chance to validate their info
            _memoryCache.SetForChallenge(new TempUser(existing));


            // All good thus far, now we just wait on our user to validate
            return ResponseShell.Ok(new SimpleSuccess());
        }


        /// <summary>
        /// Validates if a user has entered in the proper token. See also the refresh endpoint.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("validate"), Consumes("application/json"), Produces("application/json", Type = typeof(ResponseShell<TokenInfo>))]
        [SwaggerOperation(operationId: "ValidateToken", Tags = new[] { Constants.AuthorizationTag })]
        public async Task<IActionResult> Validate([FromBody]TokenCheck model)
        {
            // Get our Saved User from Memory
            var savedUser = new CheckValidationTokenEvent().Process(_memoryCache, model);

            var validatedUser = _context.Users.FirstOrDefault(x => x.PhoneNumber == savedUser.PhoneNumber);
            if (validatedUser == null)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error("Invalid");
            }

            validatedUser.LastLogin = DateTime.Now;
            _context.SaveChanges();

            // Get our token
            var token = await _tokenService.GetToken(validatedUser);
            // All good, lets give out our token
            return ResponseShell.Ok(token);

        }
    }
}
