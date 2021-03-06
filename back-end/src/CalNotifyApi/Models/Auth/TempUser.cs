﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Interfaces;
using CalNotifyApi.Utils;
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

        [DataMember(Name = "name"),Required]
        public virtual string Name { get; set; }

        [DataMember(Name = "password"),Required, MaxLength(80)]
        public virtual string Password { get; set; }

        [DataMember(Name = "email"), MaxLength(80)]
        public virtual string Email { get; set; }


        [DataMember(Name = "id"), JsonIgnore]
        public virtual Guid Id { get; set; }


        [DataMember(Name = "phone"), MaxLength(20)]
        public virtual string PhoneNumber { get; set; }


        [DataMember(Name = "enabled_email")]
        public bool EnabledEmail { get; set; }


        [DataMember(Name = "enabled_sms")]
        public bool EnabledSms { get; set; }

        public string Token { get; set; }

        public TokenType TokenType { get; set; } = TokenType.EmailToken;

       
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to provide at least an Email or Phone number.");
            }

            if (!HasAddress)
            {
                yield return new ValidationResult("Please provide an address.");
            }

            if (!string.IsNullOrEmpty(PhoneNumber) && !PhoneNumber.IsPhone())
            {
                yield return new ValidationResult("Please provide a valid phone number.");
            }

        }

        public bool HasAddress => Latitude != 0
                                  // ReSharper disable once CompareOfFloatsByEqualityOperator
                                  && Longitude != 0;

        public TempUser ShallowCopy()
        {
            return (TempUser)this.MemberwiseClone();
        }
    }

    [DataContract]
    public class ResendValidateUser : IValidatableObject
    {
        [DataMember(Name = "email"), EmailAddress]
        public virtual string Email { get; set; }

        [DataMember(Name = "phone"), Phone]
        public virtual string PhoneNumber { get; set; }

        [DataMember(Name = "id")]
        public virtual Guid Id { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to provide at least an Email or Phone number.");
            }


            if (!string.IsNullOrEmpty(PhoneNumber) && !PhoneNumber.IsPhone())
            {
                yield return new ValidationResult("Please provide a valid phone number.");
            }

        }
    }


    [DataContract]
    public class UpdateableUser : IValidatableObject
    {
       

        [DataMember(Name = "password"), MaxLength(80)]
        public virtual string Password { get; set; }

        [DataMember(Name = "email"), MaxLength(80)]
        public virtual string Email { get; set; }


        [DataMember(Name = "id")]
        public virtual Guid Id { get; set; }


        [DataMember(Name = "phone"), MaxLength(20)]
        public virtual string PhoneNumber { get; set; }


        [DataMember(Name = "enabled_email")]
        public bool EnabledEmail { get; set; }


        [DataMember(Name = "enabled_sms")]
        public bool EnabledSms { get; set; }

        public string Token { get; set; }

        /// <summary>
        ///     The addresss of the user
        /// </summary>
        [DataMember(Name = "address")]
        public Address Address { get; set; }


        public bool HasAddress
        {
            get
            {
                return  Address.GeoLocation.X != 0
                       // ReSharper disable once CompareOfFloatsByEqualityOperator
                       && Address.GeoLocation.Y != 0;
            }
        }

        public  IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
    }

    public enum TokenType
    {
        EmailToken,
        SmsToken
    }
}