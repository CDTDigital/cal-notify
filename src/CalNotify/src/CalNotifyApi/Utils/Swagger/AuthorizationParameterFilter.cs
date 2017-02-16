using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotify.Utils.Swagger
{
    public class AuthorizationParameterFilter : IOperationFilter
    {


        public void Apply(Operation operation, OperationFilterContext context)
        {
            var controllerActionDescriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null) return;

            
            var actionName = controllerActionDescriptor.ActionName;

            var resourceName = controllerActionDescriptor.ControllerName.TrimEnd('s');

            // Policy names map to scopes
            var controllerScopes = context.ApiDescription.ControllerAttributes()
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy);

            var actionScopes = context.ApiDescription.ActionAttributes()
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy);

            var allowedAnon = context.ApiDescription.ActionAttributes().OfType<AllowAnonymousAttribute>().Any();

            var requireScopes = controllerScopes.Union(actionScopes).Distinct();

            if (!allowedAnon)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<IParameter>();
                SwaggerUtils.CheckSet(operation, Constants.StatusCodes.AuthorizationErrorStatusCode.ToString(), new Response
                {
                    Description = Constants.Messages.UnauthorizedMsg
                });
                     
              
            }        
        }
    }
}
