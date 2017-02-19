using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Events.Exceptions;
using CalNotifyApi.Events.Simple;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Services;
using CalNotifyApi.Utils;
using Microsoft.AspNetCore.Identity;

namespace CalNotifyApi.Events
{
    public class UpdateUserEvent : UserRelatedEvent
    {
        [Obsolete(Constants.ObsoleteOnlyForBinding)]
        public UpdateUserEvent() { }
        public UpdateUserEvent(string customerId) : base(customerId)
        {
        }


        public async Task<GenericUser> Process(BusinessDbContext context, ValidationSender validationSender)
        {
            var user = GetUser(context);

            if (user == null)
            {
                throw new ProcessEventException(Constants.Messages.UserNotFound);
            }

            if (!string.IsNullOrWhiteSpace(Name))
                user.Name = Name;


            if (!string.IsNullOrWhiteSpace(Email) && Email != user.Email)
            {

                await validationSender.SendValidationToEmail(user);

            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber) && PhoneNumber != user.PhoneNumber)
            {
                await validationSender.SendValidationToSms(user);
            }
              
            if (string.IsNullOrWhiteSpace(NewPassowrd) || string.IsNullOrWhiteSpace(ExistingPassword))
            {
                var check = user.VerifyPassowrd(ExistingPassword);
                if (check == PasswordVerificationResult.Success)
                {
                    user.SetPassword(NewPassowrd);
                }
            }


            context.SaveChanges();
            return user;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = validationContext.GetBusinessDbContext();
            var admin = GetUser(context);
            if (admin == null)
            {
                yield return new ValidationResult(Constants.Messages.UserNotFound);
            }
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The Email of the user
        /// </summary>
        [DataMember(Name ="email"), EmailAddress]
        public string Email { get; set; }


        /// <summary>
        ///    The user's phone number which they will use to login to the service
        /// </summary>
        [DataMember(Name = "phone"), Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     The addresss of the user
        /// </summary>
        [DataMember(Name = "address")]
        public Address Address { get; set; }

        /// <summary>
        /// Exisiting password
        /// </summary>
        [DataMember(Name = "old_password")]
        public string ExistingPassword { get; set; }

        /// <summary>
        /// New password
        /// </summary>
        [DataMember(Name = "new_password")]
        public string NewPassowrd { get; set; }

    }
}
