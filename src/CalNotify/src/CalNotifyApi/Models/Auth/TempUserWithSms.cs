using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotify.Models.Interfaces;
using CalNotify.Models.User;

namespace CalNotify.Models.Auth
{
    [DataContract]
    public class TempUserWithSms : ITokenAble
    {
        public TempUserWithSms()
        {
        }

     
        public TempUserWithSms(GenericUser user)
        {
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            Token = user.Token;
        }

        public TempUserWithSms(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            PhoneNumber = phone;
        }

        [DataMember(Name = "name")]
        [Required]
        public virtual string Name { get; set; }


        [DataMember(Name = "email")]
        [EmailAddress]
        [Required]
        public virtual string Email { get; set; }


        [DataMember(Name = "phone")]
        [Required]
        [Phone]
        public virtual string PhoneNumber { get; set; }


        // Do not want this to be going across the wire, but still need
        // it to be cached for validation of users phone numbers


        public string Token { get; set; }
    }
}