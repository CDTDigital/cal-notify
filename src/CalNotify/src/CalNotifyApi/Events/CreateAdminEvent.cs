using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using CalNotify.Models.Admins;
using CalNotify.Services;
using CalNotify.Utils;
using Serilog;

namespace CalNotify.Events
{
    [DataContract]
    public class CreateAdminEvent : IValidatableObject
    {
        /// <summary>
        /// Gets or Sets Name
        /// </summary>
        [DataMember(Name = "name")]
        [Required]
        public string Name { get; set; }

        [DataMember(Name = "email"), EmailAddress, Required]
        public string Email { get; set; }



        [DataMember(Name = "password"), Required]
        public string Password { get; set; }


        [DataMember(Name = "phone"), Required, Phone]
        public string PhoneNumber { get; set; }



        public WebAdmin Process(BusinessDbContext context)
        {

            var validatedUser = context.Users.OfType<WebAdmin>().FirstOrDefault(x => x.Email == this.Email);
            if (validatedUser != null)
            {
                return validatedUser;
            }

            validatedUser = new WebAdmin(Name, Email, Password);
            var result = context.AllUsers.Add(validatedUser);
            context.SaveChanges();
            Log.Information("Created in Admin {Admin}", validatedUser);
            return validatedUser;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = validationContext.GetBusinessDbContext();
            var validatedUser = context.Users.OfType<WebAdmin>().FirstOrDefault(x => x.Email == this.Email);
            if (validatedUser != null)
            {
                yield return new ValidationResult("An Admin with that email has already been created");
            }
        }
    }
}
