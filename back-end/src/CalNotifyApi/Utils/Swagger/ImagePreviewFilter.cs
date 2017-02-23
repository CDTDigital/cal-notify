using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Utils.Swagger
{
    public class ImagePreviewFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;

            var actionName = controllerActionDescriptor.ActionName;

            if (actionName.Contains("Image"))
            {
                operation.Produces = new List<string>{"image/png"};
            }
        }
    }
}
