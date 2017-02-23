using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace CalNotifyApi.Models
{
    /// <summary>
    /// The primary way in which we store users across our system.
    /// </summary>
    [DataContract]
    public class GenericUser : BaseUser, IValidatableObject
    {


   

        /// <summary>
        ///     The addresss of the user
        /// </summary>
        [DataMember(Name = "address")]
        public  Address Address { get; set; }

      

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(PhoneNumber))
            {
                yield return new ValidationResult("Need to prodivde atleast an Email of Phone number.");
            }
        }


    }
}