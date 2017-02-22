using System.Runtime.Serialization;

namespace CalNotifyApi.Models.Auth
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class AuthRequest
    {
        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}