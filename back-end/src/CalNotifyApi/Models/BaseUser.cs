using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CalNotifyApi.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace CalNotifyApi.Models
{
    [DataContract]
    public class BaseUser : IUserIdentity
    {
        /// <summary>
        /// The unique identifier for the user. Used throughout the system!
        /// </summary>
        [DataMember(Name = "id")]
        [JsonProperty(Order = -1)]
        public System.Guid Id { get; set; }

        [DataMember(Name = "id"), JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        ///    The user's provided name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Used for validation and verification of user's when signing in.
        /// It is not persisted nor sent back to the clients, ever!
        /// </summary>
        [IgnoreDataMember, JsonIgnore, NotMapped]
        public string Token { get; set; }

        /// <summary>
        ///    The user's phone number which they will use to login to the service
        /// </summary>
        [DataMember(Name = "phone"), Phone]
        public string PhoneNumber { get; set; }

        /// <summary>
        ///    The user's email
        /// </summary>
        [DataMember(Name = "email"), EmailAddress]
        public string Email { get; set; }

        [DataMember(Name = "enabled"), JsonIgnore, JsonProperty]
        public bool Enabled { get; set; }

        [DataMember(Name = "validated_email")]
        public bool ValidatedEmail { get; set; }

        [DataMember(Name = "enabled_email")]
        public bool EnabledEmail { get; set; }

        [DataMember(Name = "validated_sms")]
        public bool ValidatedSms { get; set; }

        [DataMember(Name = "enabled_sms")]
        public bool EnabledSms { get; set; }


        /// <summary>
        ///   A user's avatar, if available
        /// Will not be passed back to clients. Instead they have to hit a dedicated endpoint
        /// </summary>
        [DataMember(Name = "avatar"), JsonIgnore]
        public byte[] Avatar { get; set; }


        /// <summary>
        /// The date at which this user signed up for the service
        /// </summary>
        [DataMember(Name = "join_date")]
        public DateTime JoinDate { get; set; }


        [DataMember(Name = "last_login")]
        public DateTime LastLogin { get; set; }

        [DataMember(Name = "password"), JsonIgnore]
        public string Password { get; set; }


        public void SetPassword(string password)
        {
            Password = PasswordHasher.HashPassword(Id.ToString(), password);
        }


        public PasswordVerificationResult VerifyPassowrd(string password)
        {
            return PasswordHasher.VerifyHashedPassword(Id.ToString(), Password, password);
        }

        private static readonly PasswordHasher<string> PasswordHasher = new PasswordHasher<string>();

    }
}
