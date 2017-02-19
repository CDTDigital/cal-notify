using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CalNotifyApi.Models.Auth
{
    [DataContract]
    public class RefreshTempUser : Interfaces.ITempUser
    {
        public RefreshTempUser()
        {
        }

     

        public RefreshTempUser(GenericUser user)
        {
            PhoneNumber = user.PhoneNumber;
            Token = user.Token;
        }

        public RefreshTempUser(string phone)
        {
            PhoneNumber = phone;
        }


        [DataMember(Name = "phone")]
        [Phone]
        public virtual string PhoneNumber { get; set; }


        // Do not want this to be going across the wire, but still need
        // it to be cached for validation of users phone numbers


        public string Token { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }
    }
}