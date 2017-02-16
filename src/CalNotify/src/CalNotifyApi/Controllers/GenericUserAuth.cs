using System.Threading.Tasks;
using CalNotify.Events;
using CalNotify.Models.Auth;
using CalNotify.Models.Responses;
using CalNotify.Models.Services;
using CalNotify.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        private readonly ISmsSender _smsSender;
        private readonly IHostingEnvironment _hostingEnvironment;


        // Tokens 
        private readonly TokenService _tokenService;
      


        public GenericUserAuth(
            BusinessDbContext context,
            ISmsSender smsSender,
            ILogger<GenericUserAuth> logger,
            ITokenMemoryCache memoryCache,
             
              TokenService tokenService,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _smsSender = smsSender;
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

        public async Task<IActionResult> CreateWithChallenge([FromBody] TempUserWithSms model)
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

            // Fire off our validation, and we set our token
            var token = await _smsSender.SendValidationToken(model);
            model.Token = token;
            // Hold our token and model for a while to give our user a chance to validate their info
            _memoryCache.SetForChallenge(model);

            // All good thus far, now we just wait on our user to validate
            return ResponseShell.Ok();
        }


        /// <summary>
        /// Validates the sms token provided in the earlier create call, and returns tokens, with a newly created GenericUser id
        /// </summary>
        /// <remarks>
        /// If the code entered is correct then a new GenericUser will be created.
        /// We are validating the token and input, and with a successful validation we go ahead and create a new GenericUser, and setup other infrastructure to support them, such as a stripe GenericUser, etc..
        /// Once you get back a token, [see an example of using this token to authorize swagger](token_example.gif).
        /// </remarks>
        /// <param name="model">GenericUser object to be created.</param>    
        [HttpPost("validate"), Consumes("application/json"), Produces("application/json", Type = typeof(ResponseShell<TokenInfo>))]
        [SwaggerOperation("ValidateToken", Tags = new[] { Constants.GenericUserEndpoint })]
        public async Task<IActionResult> Validate([FromBody]TokenCheck model)
        {
            // Get our Saved User from Memory
            var savedUser = new CheckValidationTokenEvent().Process(_memoryCache, model);
            // Create our GenericUser if need be
            var createdGenericUser =  new CreateUserEvent().Process(_context, savedUser);
            // Get our token
            var token = await _tokenService.GetToken(createdGenericUser);

            _context.SaveChanges();
            // All good, lets give out our token
            return ResponseShell.Ok(token);

        }

    }
}
