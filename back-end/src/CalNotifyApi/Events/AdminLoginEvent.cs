using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
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
            model.Default = JsonConvert.SerializeObject( new AdminLoginEvent()
            {
                Email = Constants.Testing.TestAdmins.First().Email,
                Password = Constants.Testing.TestAdmins.First().Password
            }, Constants.CreateJsonSerializerSettings());
        }
    }



    [DataContract]
    [SwaggerSchemaFilter(typeof(ExampleUserLogin))]
    public class UserLoginEvent
    {

        /// <summary>
        /// The admin's email addresss
        /// </summary>
        [DataMember(Name = "contact_info"), Required]
        public string ContactInfo { get; set; }


        /// <summary>
        /// The admin's password
        /// </summary>
        [DataMember(Name = "password"), Required]
        public string Password { get; set; }
    }


    public class ExampleUserLogin : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {

            model.Default = JsonConvert.SerializeObject(new AdminLoginEvent()
            {
                Email = Constants.Testing.TestUsers.First().Email,
                Password = Constants.Testing.DefaultUserPass
            }, Constants.CreateJsonSerializerSettings());
        }
    }
}
