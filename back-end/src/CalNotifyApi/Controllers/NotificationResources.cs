using System.Linq;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Controllers
{
    [Authorize(Constants.AuthPolicy)]
    [Authorize(Constants.AdminRole)]
    [Route(Constants.V1Prefix + "/" + Constants.NotificationEndpoint)]
    public class NotificationResources : Controller
    {
        private readonly BusinessDbContext _context;
        private readonly ILogger<NotificationResources> _logger;
        public NotificationResources(BusinessDbContext context, ILogger<NotificationResources> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates a notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("")]
        [SwaggerOperation("CREATE_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<Notification>), 200)]
        public virtual IActionResult CreateNotification([FromBody] CreateNotificationEvent model)
        {
            var adminId =HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == Constants.UserIdClaimKey);
            var notification = model.Process(_context, adminId?.Value);
            return ResponseShell.Ok(notification);
        }


        /// <summary>
        /// Gets a notifications
        /// </summary>
        [HttpGet("{id}")]
        [SwaggerOperation("GET_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<Notification>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult GetNotification([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);
            return ResponseShell.Ok(notification);
        }


        [HttpPut("{id}")]
        [SwaggerOperation("BROADCAST_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<SimpleSuccess>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult BroadcastNotification([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);
            new BroadcastNotificationEvent();
            return ResponseShell.NotImplementated();
        }
    }

    /// <summary>
    /// </summary>
    public class ValidateNotificationExists : TypeFilterAttribute
    {
        /// <summary>
        /// </summary>
        public ValidateNotificationExists() : base(typeof
            (ValidateImpl))
        {
        }

        private class ValidateImpl : IAsyncActionFilter
        {
            private readonly BusinessDbContext _context;

            public ValidateImpl(BusinessDbContext authorRepository)
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
                        var check = await _context.Notifications.FirstOrDefaultAsync(u => u.Id.ToString() == id);
                        if (check == null)
                        {
                            context.Result = ResponseShell.NotFound(Constants.Messages.NotificationNotFound);
                            return;
                        }
                    }
                }
                await next();
            }
        }
    }

}
