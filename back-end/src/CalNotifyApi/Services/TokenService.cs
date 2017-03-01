using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Admins;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Interfaces;

namespace CalNotifyApi.Services
{
    /// <summary>
    /// Provides the services required to create the full token required for all api endpoints.
    /// Pulls in other services and dependencies such as our GeoRealTime and StripeService.
    /// TODO: Catch errors if firebase token generation fails.
    /// TODO: Pull configuration from a unique api configuration class
    /// </summary>
    public class TokenService
    {

        private readonly TokenAuthOptions _options;
        private readonly ExternalServicesConfig _config;


        public TokenService(TokenAuthOptions options, ExternalServicesConfig configuration)
        {
            _options = options;
            _config = configuration;

          
        }

     

        public Claim RoleClaim(IUserIdentity user)
        {
            string roleClaim = "";

            if (user is GenericUser)
            {
                roleClaim = Constants.UserRole;
            }
            else if (user is WebAdmin)
            {
                roleClaim = Constants.AdminRole;
            }
            return new Claim(Constants.RoleClaimKey, roleClaim);
        }

        public async Task<TokenInfo> GetToken(IUserIdentity user)
        {
            var handler = new JwtSecurityTokenHandler();
            var expires = DateTimeOffset.UtcNow.AddDays(Constants.ExpirationOffsetInDays);

            // For now, just creating a simple generic identity.
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.UserName, "TokenAuth"),
                new[]
                {
                    new Claim(Constants.UserIdClaimKey, user.Id.ToString(), ClaimValueTypes.String),
                    this.RoleClaim(user)

                });

            var securityToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor()
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = _options.SigningCredentials,
                Subject = identity,
                Expires = expires.Date
            });


            return new TokenInfo()
            {
                Expires = expires.ToUnixTimeSeconds(),
                Token = handler.WriteToken(securityToken),        
               
                UserId = user.Id.ToString()
            };
        }
    }
}
