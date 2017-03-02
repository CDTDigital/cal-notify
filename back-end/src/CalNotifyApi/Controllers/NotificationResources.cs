using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalNotifyApi.Events;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Models.Services;
using CalNotifyApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfigurationRoot _configuration;

        public NotificationResources(BusinessDbContext context, ILogger<NotificationResources> logger, ValidationSender validation, IHostingEnvironment hostingEnvironment, IConfigurationRoot configuration)
        {
            _context = context;
            _logger = logger;
            _validation = validation;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a notification
        /// </summary>
        /// <param name="model">The notification details and information to create in our database</param>
        /// <returns></returns>
        [HttpPut("")]
        [SwaggerOperation("CREATE_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<Notification>), 200)]
        public virtual IActionResult CreateNotification([FromBody] CreateNotificationEvent model)
        {
            var adminId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == Constants.UserIdClaimKey);
            
            var notification = model.Process(_context, adminId.Value);
            return ResponseShell.Ok(notification);
        }


        /// <summary>
        /// Lists out all notifications
        /// </summary>
        [HttpGet("")]
        [SwaggerOperation("LIST_NOTIFICATION", Tags = new[] { Constants.NotificationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<List<Notification>>), 200)]
        public virtual IActionResult ListNotifications()
        {
            var list = _context.Notifications.OrderBy(x=>x.Created).ToList();
            return ResponseShell.Ok(list);
        }

        /// <summary>
        /// Gets a single notification
        /// </summary>
        /// <param name="id">The id of the notification</param>
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
        /// <param name="id">The id of the notification</param>
        [HttpPut("{id}")]
        [SwaggerOperation("PUBLISH_NOTIFICATION", Tags = new[] { Constants.NotificationEndpoint })]
        [ProducesResponseType(typeof(ResponseShell<Notification>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult PublishNotification([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);
            var adminId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == Constants.UserIdClaimKey);
            var connectionString = _configuration.GetConnectionString(_hostingEnvironment.EnvironmentName);

            notification = new PublishNotificationEvent().Process(_context, adminId.Value, _validation, connectionString, notification);

            return ResponseShell.Ok(notification);
        }


        /// <summary>
        /// Get the log of sent messages for a notiification
        /// </summary>
        /// <param name="id">The id of the notification</param>
        [HttpGet("{id}/log")]
        [SwaggerOperation("BROADCAST_NOTIFICATION", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<List<PublishedNotificationLog>>), 200)]
        [ValidateNotificationExists]
        public virtual IActionResult NotificationLog([FromRoute] long id)
        {
            var notification = _context.Notifications.FirstOrDefault(n => n.Id == id);

            var sentusers =
                _context.NotificationLog.Where(x => x.NotificationId == notification.Id).Select(x => x.UserId);

            var locations =
                _context.Users.Where(x => sentusers.Contains(x.Id))
                        .Select(x => x.Address.GeoLocation)
                        .Select(p => new GeoLocation(p.Y, p.X)).ToList();
            var log = new PublishedNotificationLog()
            {
             SentEmail  = _context.NotificationLog.Count(x => x.NotificationId == notification.Id && x.Type == LogType.Email),
             SentSms =   _context.NotificationLog.Count(x => x.NotificationId == notification.Id && x.Type == LogType.Sms),
             Published = notification.Published.GetValueOrDefault(),
             SentLocations = locations
            };
            
            return ResponseShell.Ok(log);

        }

        /// <summary>
        /// Pulls and updates our database with notifications from external USGS and NOAA sources
        /// </summary>
        /// <remarks>
        /// For this phase of the prototype we have this url public to allow an easy means of periodic updating. 
        /// It acts like the eventuall webhook it will become.
        /// </remarks>
        /// <returns></returns>
        [HttpGet("pull")]
        [SwaggerOperation("UPDATE_SOURCES", Tags = new[] {Constants.NotificationEndpoint})]
        [ProducesResponseType(typeof(ResponseShell<SimpleSuccess>), 200)]
        [AllowAnonymous]
        public virtual async Task<IActionResult> UpdateSources()
        {
            await new PullFromSourcesEvent().Process(_context);
            return ResponseShell.Ok();
        }
    }

    /// <summary>
    /// Validate that the notification exists in our database
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
