using Microsoft.IdentityModel.Tokens;

namespace CalNotify.Models.Auth
{
    public class TokenAuthOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public RsaSecurityKey Key { get; set; }
    }
}