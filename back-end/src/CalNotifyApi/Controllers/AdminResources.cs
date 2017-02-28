using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Controllers
{
    [Authorize(Constants.AuthPolicy)]
    [Authorize(Constants.AdminRole)]
    [Route(Constants.V1Prefix + "/" + Constants.AdminConfigurationEndpoint)]
    public class AdminResources : Controller
    {
        private readonly BusinessDbContext _context;
        private readonly ILogger<AdminResources> _logger;
        private readonly ExternalServicesConfig _config;

        private readonly TokenService _tokenService;

        public AdminResources(BusinessDbContext context, ILogger<AdminResources> logger, TokenService tokenService, ExternalServicesConfig config)
        {
            _context = context;
            _logger = logger;

            _tokenService = tokenService;
            _config = config;
        }

        /// <summary>
        /// Retrieves the current system configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet("config")]
        [SwaggerOperation("GET_SYSTEM_CONFIGURATION", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<AdminConfiguration>), 200)]
        public virtual IActionResult GetConfiguration()
        {
            return ResponseShell.Ok(_context.AdminConfig);
        }


        /// <summary>
        /// Updates the system configuration
        /// </summary>
        /// <param name="model">The phone number to update</param>
        [HttpPost("config")]
        [SwaggerOperation("UPDATE_SYSTEM_CONFIGURATION", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<SimpleSuccess>), 200)]
        public virtual IActionResult UpdateConfiguration()
        {

            return ResponseShell.NotImplementated();
        }



        /// <summary>
        /// Creates a new admin
        /// </summary>
        /// <param name="model">Admin to create</param>
        /// <returns></returns>
        [HttpPost("create")]
        [SwaggerOperation("CREATE_ADMIN_ENDPOINT", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<WebAdmin>), 200)]
        public virtual IActionResult CreateAdmin([FromBody] CreateAdminEvent model)
        {
            var newAdmin = model.Process(_context);
            return ResponseShell.Ok(newAdmin);
        }




        /// <summary>
        /// Allows an admin to login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [SwaggerOperation("ADMIN_LOGIN", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<TokenInfo>), 200)]
        [AllowAnonymous] // need to allow login from an unathenticated state
        public virtual async Task<IActionResult> AdminLogin([FromBody] AdminLoginEvent model)
        {
            // The eventual user

            var validatedUser = _context.Admins.OfType<WebAdmin>().FirstOrDefault(x => x.Email.ToLower() == model.Email.ToLower());
            if (validatedUser == null)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error("Invalid");
            }
            var passCheck = validatedUser.VerifyPassowrd(model.Password);
            if (passCheck != PasswordVerificationResult.Success)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error("Invalid");
            }

            // Get our token
            var token = await _tokenService.GetToken(validatedUser);

            return ResponseShell.Ok(token);
            // return Redirect($"{_config.Urls.Frontend}/{_config.Pages.AdminPage}?user={token.UserId}&token={token.Token}");

        }



        /// <summary>
        /// Lists out all admins
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [SwaggerOperation("GET_ALL_ADMINS", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<List<WebAdmin>>), 200)]

        public virtual IActionResult ListOfAdmins()
        {
            return ResponseShell.Ok(_context.Admins.ToList());
        }


        /// <summary>
        /// Get an admin by their id
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [SwaggerOperation("GET_ADMIN_BY_ID", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<WebAdmin>), 200)]
        public virtual IActionResult AdminById([FromRoute] string id)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.Id.ToString() == id);
            if (admin == null)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error(Constants.Messages.UserNotFound);
            }
            return ResponseShell.Ok(admin);
        }


        /// <summary>
        /// Updates an admins properties
        /// </summary>
        /// 
        /// <returns>The updated admin</returns>
        [HttpPut("")]
        [SwaggerOperation("UPDATE_ADMIN_BY_ID", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<WebAdmin>), 200)]
        public virtual IActionResult UpdateAdminById([FromBody] AdminUpdateEvent model)
        {
            model.Process(_context);
            return ResponseShell.Ok(model.GetAdmin(_context));
        }

        /// <summary>
        /// Delete an admin by id
        /// </summary>
        /// <param name="id">The admin to remove</param>
        [HttpDelete("{id}")]
        [SwaggerOperation("DELETE_ADMIN_BY_ID", Tags = new[] { Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<SimpleSuccess>), 200)]
        public virtual IActionResult DeleteAdminById([FromRoute] string id)
        {

            var admin = _context.Admins.FirstOrDefault(x => x.Id.ToString() == id);
            if (admin == null)
            {
                // Our response is vague to avoid leaking information
                return ResponseShell.Error(Constants.Messages.UserNotFound);
            }

            _context.AllUsers.Remove(admin);
            _context.SaveChanges();

            return ResponseShell.Ok();

        }
    }




}
