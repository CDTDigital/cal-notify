using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalNotify.Events.Simple;
using CalNotify.Models.Responses;
using CalNotify.Services;
using CalNotify.Models;
using CalNotify.Models.User;
using CalNotifyApi.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotify.Controllers.GenericUsers
{
    /// <summary>
    /// </summary>
    [Authorize(Constants.AuthPolicy)]
    [Route(Constants.V1Prefix + "/" + Constants.GenericUserEndpoint)]
    public class GenericUserResources : Controller
    {
        private readonly BusinessDbContext _context;
        private readonly ILogger<GenericUserResources> _logger;
      
  

        /// <summary>
        /// </summary>
        public GenericUserResources(BusinessDbContext context, ILogger<GenericUserResources> logger)
        {
            _context = context;
            _logger = logger;
        }



        // ENDPOINTS FOR RETRIEVING GenericUser OBJECTS
        #region retrival

        /// <summary>
        /// Retrieves a GenericUser by their unique id
        /// </summary>
        /// <param name="id">The GenericUser id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ValidateGenricExists]
        [SwaggerOperation("GET_GENERICUSER_BY_ID", Tags = new[] { Constants.GenericUserEndpoint })]
        [Produces("application/json", Type = typeof(ResponseShell<GenericUser>))]
        public virtual IActionResult GetById([FromRoute] string id)
        {       
            var user = _context.Users.FirstOrDefault(u => u.Id == new Guid(id)); 
            return ResponseShell.Ok(user);
        }


        /// <summary>
        /// Retrieves a list of all Users in the system
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet("")]
        [Produces("application/json", Type = typeof(ResponseShell<List<GenericUser>>))]
        [SwaggerOperation("GET_ALL_GENERICUSERS", Tags = new[] { Constants.GenericUserEndpoint })]
        public virtual async Task<IActionResult> GetAll([FromQuery] Pagination pagination)
        {
            var users = _context.Users.OfType<GenericUser>();
            var fetchedUsers = await pagination.SkipAndTake(users).ToListAsync();

            return ResponseShell.Ok(fetchedUsers);
        }


        [HttpDelete("{id}")]
        [ValidateGenricExists]
        [Authorize(Constants.AdminRole)] // Lock down deleting a GenericUser
        [SwaggerOperation("DELETE_GenericUser_BY_ID",
            Tags = new[] { Constants.GenericUserEndpoint, Constants.AdminConfigurationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<MaybeSuccess>), 200)]
        public virtual IActionResult RemoveById([FromRoute] string id)
        {

            var user = _context.Users.FirstOrDefault(u => u.Id == new Guid(id));
            _context.AllUsers.Remove(user);
            return ResponseShell.Ok(new MaybeSuccess() { Success = true });
        }
        #endregion

     

        // ENDPOINTS FOR UPDATING GenericUserS
        #region updates

        /// <summary>
        /// Updates a GenericUser's properties such as Name via their unique id.
        /// </summary>
        /// 
        /// <remarks>
        /// For the time being this only can update a users name
        /// </remarks>
        /// <param name="modelUpdates"></param>
        /// <param name="validationSender"></param>
        /// <returns></returns>
        [HttpPut("")]
        [SwaggerOperation("UPDATE_GENERICUSER_BY_ID", Tags = new[] { Constants.GenericUserEndpoint })]
        public virtual async Task<IActionResult> UpdateById([FromBody] UpdateUserEvent modelUpdates, [FromServices] ValidationSender validationSender)
        {
            var user = await modelUpdates.Process(_context, validationSender);
            _context.SaveChanges();
            return ResponseShell.Ok(user);  
        }

        #endregion

        // ATTRIBUTES AND OTHER HELPERS FOR SERVING UP GenericUser RESOURCES
        #region Helpers



        public class ValidateUserClaimIdAttribute : ActionFilterAttribute, IAsyncActionFilter
        {
            public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (context.ActionArguments.ContainsKey("id"))
                {
                    var id = context.ActionArguments["id"] as string;
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        var check = context.HttpContext.User.HasClaim(Constants.UserIdClaimKey, id);
                        var adminCheck = context.HttpContext.User.IsInRole(Constants.AdminRole);
                        if (!check || adminCheck)
                        {
                            context.Result = ResponseShell.AuthError(new Meta()
                            {
                                Message = Constants.Messages.InvalidClaimMsg,

                                Code = 403
                            });
                            return;
                        }
                    }
                }
                await next();
            }
        }


        /// <summary>
        /// </summary>
        public class ValidateGenricExistsAttribute : TypeFilterAttribute
        {
            /// <summary>
            /// </summary>
            public ValidateGenricExistsAttribute() : base(typeof
                (ValidateUserExistsFilterImpl))
            {
            }

            private class ValidateUserExistsFilterImpl : IAsyncActionFilter
            {
                private readonly BusinessDbContext _context;

                public ValidateUserExistsFilterImpl(BusinessDbContext authorRepository)
                {
                    _context = authorRepository;
                }

                public async Task OnActionExecutionAsync(ActionExecutingContext context,
                    ActionExecutionDelegate next)
                {
                    if (context.ActionArguments.ContainsKey("id"))
                    {
                        var id = context.ActionArguments["id"] as string;
                        if (!string.IsNullOrWhiteSpace(id))
                        {
                            var check = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == id);
                            if (check == null)
                            {
                                context.Result = ResponseShell.NotFound(Constants.Messages.UserNotFound);
                                return;
                            }
                        }
                    }
                    await next();
                }
            }
        }


        #endregion
    }
}