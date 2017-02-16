using CalNotify.Models.Responses;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CalNotify.Events.Attributes
{
    /// <summary>
    /// We deduplicate alot of logic related to bailing early and fast in the request lifecycle, 
    /// whenever model state invariants are broken
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = ResponseShell.Error(Constants.StatusCodes.ModelErrorStatusCode, Constants.Messages.InvalidModelMsg, context.ModelState);
            }
        }
    }
}
