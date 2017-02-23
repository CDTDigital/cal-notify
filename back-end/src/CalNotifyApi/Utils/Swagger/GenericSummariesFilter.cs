using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Utils.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericSummariesFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;
          
            var actionName = controllerActionDescriptor.ActionName;
           
            var resourceName = controllerActionDescriptor.ControllerName.TrimEnd('s');
           
            switch (actionName)
            {
                case "Create":
                    operation.Summary = operation.Summary ?? $"Creates a {resourceName}";

                    SwaggerUtils.CheckSet(operation, "200", new Response { Description = $"A {resourceName} was created" });
                    SwaggerUtils.CheckSet(operation, "400", new Response {Description = $"Failed creating a {resourceName}"});
                   
                    break;
                case "GetAll":

                    operation.Summary = operation.Summary ?? $"Returns all {resourceName}s";
                    break;
                case "GetById":
                    operation.Summary = operation.Summary ?? $"Retrieves a {resourceName} by unique id";
                    SwaggerUtils.CheckSet(operation, "404", new Response { Description = $"Could not find the requested {resourceName}" });
                    break;

                case "Update":
                    operation.Summary = operation.Summary ?? $"Updates a {resourceName} by unique id";
                    operation.Parameters[0].Description = $"a unique id for the {resourceName}";
                    if (operation.Parameters.Count >= 2)
                    {
                        operation.Parameters[1].Description = $"a {resourceName} representation";
                    }
                  
                    SwaggerUtils.CheckSet(operation, "404", new Response { Description = $"Could not find the requested {resourceName}" });

                    break;
                case "Delete":
                    operation.Summary = operation.Summary ?? $"Deletes a {resourceName} by unique id";
                    operation.Parameters[0].Description = $"a unique id for the {resourceName}";
                    break;
            }
        }

       
    }

    
}
