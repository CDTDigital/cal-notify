using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Interfaces;
using Newtonsoft.Json;

namespace CalNotifyApi.Models.Auth
{
    [DataContract]
    public class TempUser : AddressWithLatLng, ITempUser, IValidatableObject
    {
        public TempUser()
        {
        }

     
        public TempUser(GenericUser user)
        {
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Token = user.Token;
            Id = user.Id;
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


        [DataMember(Name = "id"), JsonIgnore]
        public virtual Guid Id { get; set; }


        [DataMember(Name = "phone")]
        public virtual string PhoneNumber { get; set; }



        public string Token { get; set; }

        public TokenType TokenType { get; set; } = TokenType.EmailToken;

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to prodivde atleast an Email of Phone number.");
            }
        }

        public TempUser ShallowCopy()
        {
            return (TempUser)this.MemberwiseClone();
        }
    }

    public enum TokenType
    {
        EmailToken,
        SmsToken
    }
}