using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Addresses;

namespace CalNotifyApi.Models.Auth
{
    [DataContract]
    public class TempUser : AddressWithLatLng, Interfaces.ITempUser, IValidatableObject
    {
        public TempUser()
        {
        }

     
        public TempUser(GenericUser user)
        {
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Token = user.Token;
        }

        public TempUser(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            PhoneNumber = phone;
        }

        [DataMember(Name = "name")]
        public virtual string Name { get; set; }


        [DataMember(Name = "email")]
        public virtual string Email { get; set; }


        [DataMember(Name = "phone")]
        public virtual string PhoneNumber { get; set; }



        public string Token { get; set; }

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to prodivde atleast an Email of Phone number.");
            }
        }
    }
}