using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotify.Models.Interfaces;
using CalNotify.Models.User;

namespace CalNotify.Models.Auth
{
    [DataContract]
    public class TempUserRefreshTempUserSmsWithSms : ITokenAble
    {
        public TempUserRefreshTempUserSmsWithSms()
        {
        }

     

        public TempUserRefreshTempUserSmsWithSms(GenericUser user)
        {
            PhoneNumber = user.PhoneNumber;
            Token = user.Token;
        }

        public TempUserRefreshTempUserSmsWithSms(string phone)
        {
            PhoneNumber = phone;
        }


        [DataMember(Name = "phone")]
        [Required]
        [Phone]
        public virtual string PhoneNumber { get; set; }


        // Do not want this to be going across the wire, but still need
        // it to be cached for validation of users phone numbers


        public string Token { get; set; }
    }
}