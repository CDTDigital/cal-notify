using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CalNotify.Models.Addresses;
using CalNotify.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace CalNotify.Models.User
{
    /// <summary>
    /// The primary way in which we store users across our system.
    /// </summary>
    [DataContract]
    public class GenericUser :  ITokenAble, IUserIdentity
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
        ///   A user's avatar, if available
        /// Will not be passed back to clients. Instead they have to hit a dedicated endpoint
        /// </summary>
        [DataMember(Name = "avatar"), JsonIgnore]
        public byte[] Avatar { get; set; }


        /// <summary>
        ///    The user's phone number which they will use to login to the service
        /// </summary>
        [DataMember(Name = "phone")]
        public  string PhoneNumber { get; set; }

        /// <summary>
        /// Used for validation and verification of user's when signing in.
        /// It is not persisted nor sent back to the clients, ever!
        /// </summary>
        [IgnoreDataMember, JsonIgnore, NotMapped]
        public string Token { get; set; }


        [DataMember(Name ="enabled"), JsonIgnore, JsonProperty]
        public string Enabled { get; set; }
      
        /// <summary>
        ///    The user's email
        /// </summary>
        [DataMember(Name = "email")]
        public  string Email { get; set; }

        /// <summary>
        /// The date at which this user signed up for the service
        /// </summary>
        [DataMember(Name = "join_date")]
        public DateTime JoinDate { get; set; }


        [DataMember(Name = "last_login")]
        public DateTime LastLogin { get; set; }


        /// <summary>
        ///     The addresss of the user
        /// </summary>
        [DataMember(Name = "address")]
        public  Address Address { get; set; }



        [DataMember(Name = "password"), JsonIgnore]
        public string Password { get; set; }


        public void SetPassword(string password)
        {
            Password = PasswordHasher.HashPassword(UserName, password);
        }


        public PasswordVerificationResult VerifyPassowrd(string password)
        {
            return PasswordHasher.VerifyHashedPassword(UserName, Password, password);
        }

        private static readonly PasswordHasher<string> PasswordHasher = new PasswordHasher<string>();
    }
}