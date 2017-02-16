using Swashbuckle.AspNetCore.Swagger;

namespace CalNotify.Utils.Swagger
{
    public class SwaggerUtils
    {
        public static void CheckSet(Operation operation, string code, Response response)
        {
            if (operation.Responses.ContainsKey(code))
            {
                operation.Responses[code].Description = response.Description;
            }
            else
            {
                operation.Responses.Add(code, response);
            }
        }
    }
}
