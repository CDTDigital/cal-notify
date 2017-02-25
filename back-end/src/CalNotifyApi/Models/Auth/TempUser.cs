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

        [DataMember(Name = "password"), Required]
        public virtual string Password { get; set; }

        [DataMember(Name = "email")]
        public virtual string Email { get; set; }


        [DataMember(Name = "id"), JsonIgnore]
        public virtual Guid Id { get; set; }


        [DataMember(Name = "phone")]
        public virtual string PhoneNumber { get; set; }


        [DataMember(Name = "enabled_email")]
        public bool EnabledEmail { get; set; }


        [DataMember(Name = "enabled_sms")]
        public bool EnabledSms { get; set; }

        public string Token { get; set; }

        public TokenType TokenType { get; set; } = TokenType.EmailToken;

       
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to provide at least an Email or Phone number.");
            }

            if (!HasAddress)
            {
                yield return new ValidationResult("Please provide an address.");
            }


        }

        public bool HasAddress
        {
            get
            {
                return !string.IsNullOrEmpty(City)
                       && !string.IsNullOrEmpty(State)
                       && !string.IsNullOrEmpty(Street)
                       && !string.IsNullOrEmpty(Zip)
                       // ReSharper disable once CompareOfFloatsByEqualityOperator
                       && Latitude != 0
                       // ReSharper disable once CompareOfFloatsByEqualityOperator
                       && Longitude != 0;
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