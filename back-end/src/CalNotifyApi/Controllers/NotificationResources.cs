using System.Collections.Generic;
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
        private readonly ValidationSender _validation;
        public NotificationResources(BusinessDbContext context, ILogger<NotificationResources> logger, ValidationSender validation)
        {
            _context = context;
            _logger = logger;
            _validation = validation;
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
        /// Lists out notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("")]
        [SwaggerOperation("LIST_NOTIFICATION", Tags = new[] { Constants.NotificationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<List<Notification>>), 200)]
        public virtual IActionResult ListNotifications()
        {
            var list = _context.Notifications.ToList();
            return ResponseShell.Ok(list);
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


        /// <summary>
        /// Update a notifications
        /// </summary>
        [HttpPatch("{id}")]
        [SwaggerOperation("UPDATE_NOTIFICATION", Tags = new[] { Constants.NotificationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<Notification>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult UpdateNotification([FromRoute] long id, [FromBody] CreateNotificationEvent model)
        {
            var notification =  model.UpdateProcess(_context, id);
            return ResponseShell.Ok(notification);
        }

        /// <summary>
        /// Broadcasts the notification to the affected users
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerOperation("PUBLISH_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<SimpleSuccess>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult PublishNotification([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);
            var adminId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == Constants.UserIdClaimKey);
#pragma warning disable 4014
            new PublishNotificationEvent().Process(_context, adminId.Value, _validation, notification);
#pragma warning restore 4014
            return ResponseShell.Ok();
        }
        /// <summary>
        /// Get the log of sent messages for a notiification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/log")]
        [SwaggerOperation("BROADCAST_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<List<BroadCastLogEntry>>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult NotificationLog([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);
            var list = _context.NotificationLog.Where(x => x.NotificationId == notification.Id.Value);
            return ResponseShell.Ok(list);

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
