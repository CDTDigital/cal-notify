using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotify.Models.Responses
{
    public class ExampleMetaObject: ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            model.Default = new Meta()
            {
                Code = 200,
            };
        }
    }
}
