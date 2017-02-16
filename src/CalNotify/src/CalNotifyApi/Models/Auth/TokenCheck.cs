using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotify.Models.Interfaces;

namespace CalNotify.Models.Auth
{
    [DataContract]
    public struct TokenCheck : ITokenAble
    {
        // Making the token required
        [Required]
        [DataMember(Name = "token")]
        public string Token { get; set; }

        [Required]
        [DataMember(Name = "phone")]
        public string PhoneNumber { get; set; }
    }
}