using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CalNotifyApi.Models.Interfaces;

namespace CalNotifyApi.Models.Auth
{
    [DataContract]
    public struct TokenCheck : ITempUser
    {
        // Making the token required
        [Required]
        [DataMember(Name = "token")]
        public string Token { get; set; }

        [DataMember(Name = "name")]
        public  string Name { get; set; }
      
        [DataMember(Name = "phone")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }


        public TokenType TokenType { get; set; }
    }
}