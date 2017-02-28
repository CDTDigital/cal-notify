using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Controllers
{
    /// <summary>
    /// </summary>
    [Authorize(Constants.AuthPolicy)]
    [Route(Constants.V1Prefix + "/" + Constants.GenericUserEndpoint)]
    public class GenericUserResources : Controller
    {
        private readonly BusinessDbContext _context;
        private readonly ILogger<GenericUserResources> _logger;

        private readonly ValidationSender _validation;

        private readonly ITokenMemoryCache _memoryCache;


        /// <summary>
        /// </summary>
        public GenericUserResources(BusinessDbContext context, ILogger<GenericUserResources> logger, ValidationSender validation, ITokenMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _validation = validation;
            _memoryCache = memoryCache;
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
        [Authorize(Constants.AdminRole)]
        [Produces("application/json", Type = typeof(ResponseShell<List<GenericUser>>))]
        [SwaggerOperation("GET_ALL_GENERICUSERS", Tags = new[] { Constants.GenericUserEndpoint })]
        public virtual async Task<IActionResult> GetAll([FromQuery] Pagination pagination)
        {
            var users = _context.Users.OfType<GenericUser>();
            var fetchedUsers = await pagination.SkipAndTake(users).ToListAsync();

            return ResponseShell.Ok(fetchedUsers);
        }


        /// <summary>
        /// Updates a GenericUser's properties such as Email or Sms via their unique id.
        /// </summary>
        /// 
        [HttpPut("")]
        [SwaggerOperation("UPDATE_GENERICUSER_BY_ID", Tags = new[] { Constants.GenericUserEndpoint })]
        public virtual async Task<IActionResult> UpdateById([FromBody] UpdateableUser tempUser)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {

                    var user = _context.Users.FirstOrDefault(u => u.Id == tempUser.Id);
                    if (user == null)
                    {
                        // Our response is vague to avoid leaking information
                        return ResponseShell.Error("Could not find an account with that information");
                    }


                   
                  
                    user.EnabledEmail = tempUser.EnabledEmail;
                    user.EnabledSms = tempUser.EnabledSms;

                    _context.Address.Update(user.Address);

                    user.Address.Zip = tempUser.Address.Zip;
                    user.Address.City = tempUser.Address.City;
                    user.Address.FormattedAddress = tempUser.Address.FormattedAddress;
                    user.Address.Number = tempUser.Address.Number;
                    user.Address.State = tempUser.Address.State;
                    tempUser.Address.GeoLocation.SRID = Constants.SRID;
                    user.Address.GeoLocation = tempUser.Address.GeoLocation;


                    if (!string.IsNullOrWhiteSpace(tempUser.Email) && tempUser.Email != user.Email)
                    {
                        user.Email = tempUser.Email;
                        Log.Information("Sending Email Validation to {user}", tempUser);
                        var temp = new TempUser(user);
                        await _validation.SendValidationToEmail(temp);
                        _memoryCache.SetForChallenge(temp);

                    }

                    if (!string.IsNullOrWhiteSpace(tempUser.PhoneNumber) && tempUser.PhoneNumber != user.PhoneNumber)
                    {
                        user.PhoneNumber = tempUser.PhoneNumber;
                        Log.Information("Sending SMS Validation to {user}", tempUser);
                        var temp = new TempUser(user);
                        await _validation.SendValidationToSms(temp);
                        _memoryCache.SetForChallenge(temp);
                    }

                    if (!string.IsNullOrWhiteSpace(tempUser.Password))
                    {

                        user.SetPassword(tempUser.Password);
                    }
                    transaction.Commit();
                    _context.SaveChanges();
                    return ResponseShell.Ok(user);
                }

            }
            catch (Exception e)
            {
                Log.Error(e, "Could not update user details");
                throw;
            }

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