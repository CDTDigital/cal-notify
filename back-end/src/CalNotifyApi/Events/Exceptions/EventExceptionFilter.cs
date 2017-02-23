using CalNotifyApi.Models.Responses;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace CalNotifyApi.Events.Exceptions
{
    public class EventExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {

            if (context.Exception is IProcessEventException)
            {
                // handle explicit 'known'  errors
                var ex = context.Exception as IProcessEventException;
                Log.Error(context.Exception, ex.Message);
                context.HttpContext.Response.StatusCode = Constants.StatusCodes.EventErrorStatusCode;
                context.Result = ex.ResponseShellError;
            }
            else
            {
                Log.Error(context.Exception, "An unxecpeted related exception occured");
                context.HttpContext.Response.StatusCode = Constants.StatusCodes.ErrorUnexpcetedStatusCode;
                context.Result = ResponseShell.ErrorUnexpected(context.Exception);
            }
            // clear it 
            context.Exception = null;
        }
    }

  
}
