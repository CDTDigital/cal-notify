using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Events.Simple;
using CalNotifyApi.Services;
using CalNotifyApi.Utils;
using Microsoft.AspNetCore.Identity;

namespace CalNotifyApi.Events
{
    public class AdminUpdateEvent: AdminRelatedEvent, IValidatableObject
    {
        /// <summary>
        ///     MVC Model binding requires an empty constructor in order to properly work.
        /// </summary>
        [Obsolete(Constants.ObsoleteOnlyForBinding, true)]
        public AdminUpdateEvent() { }

        /// <summary>
        ///     Initializes our event with the needed admin id.
        /// </summary>
        /// <param name="adminId">The id of the admin this event needs</param>
        public AdminUpdateEvent(string adminId): base(adminId)
        {
 
        }

        public void Process(BusinessDbContext context)
        {
            var admin = GetAdmin(context);

            if (admin == null)
            {
                throw new ProcessEventException(Constants.Messages.UserNotFound);
            }

            if (!string.IsNullOrWhiteSpace(Name))
                admin.Name = Name;


            if (!string.IsNullOrWhiteSpace(Email))
                admin.Email = Email;

            if (string.IsNullOrWhiteSpace(NewPassowrd) || string.IsNullOrWhiteSpace(ExistingPassword))
            {
                var check = admin.VerifyPassowrd(ExistingPassword);
                if (check == PasswordVerificationResult.Success)
                {
                    admin.SetPassword(NewPassowrd);
                }
            }

            context.SaveChanges();

        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = validationContext.GetBusinessDbContext();
            var admin = GetAdmin(context);
            if (admin == null)
            {
                yield return new ValidationResult(Constants.Messages.UserNotFound);
            }
        }


        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name ="email"), EmailAddress]
        public string Email { get; set; }

        [DataMember(Name="old_password")]
        public string ExistingPassword { get; set; }

        [DataMember(Name="new_password")]
        public string NewPassowrd { get; set; }
    }
}
