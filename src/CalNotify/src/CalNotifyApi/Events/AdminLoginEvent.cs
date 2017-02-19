using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalNotifyApi.Events
{
    [DataContract]
    [SwaggerSchemaFilter(typeof(ExampleAdminLogin))]
    public class AdminLoginEvent
    {

        /// <summary>
        /// The admin's email addresss
        /// </summary>
        [DataMember(Name = "email"), Required]
        public string Email { get; set; }


        /// <summary>
        /// The admin's password
        /// </summary>
        [DataMember(Name = "password"), Required]
        public string Password { get; set; }
    }


    public class ExampleAdminLogin : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            model.Default = new AdminLoginEvent()
            {
                Email = Constants.Testing.TestAdmins.First().Email,
                Password = Constants.Testing.TestAdmins.First().Password
            };
        }
    }
}
