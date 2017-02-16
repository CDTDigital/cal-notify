using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CalNotify.Utils.Swagger
{
    public class LowerCaseQueryParametersApiDescriptionProvider : IApiDescriptionProvider
    {
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            foreach (var parameter in context.Results.SelectMany(x => x.ParameterDescriptions).Where(x => x.Source.Id == "Query"))
            {
                parameter.Name = parameter.Name.ToLower();
            }
        }
    }
}
