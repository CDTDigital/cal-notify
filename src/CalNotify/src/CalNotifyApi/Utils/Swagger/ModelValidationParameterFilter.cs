using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Utils.Swagger
{
    public class ModelValidationParameterFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            SwaggerUtils.CheckSet(operation, Constants.StatusCodes.ModelErrorStatusCode.ToString(), new Response
            {
                Description = Constants.Messages.InvalidModelMsg
            });
            SwaggerUtils.CheckSet(operation, Constants.StatusCodes.ErrorUnexpcetedStatusCode.ToString(), new Response
            {
                Description = Constants.Messages.UnexpectedErrorMsg
            });
        }

    }
}