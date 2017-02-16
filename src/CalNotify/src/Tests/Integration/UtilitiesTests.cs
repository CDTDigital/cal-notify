using System.Linq;
using System.Threading.Tasks;
using Xunit;

using CalNotify;
using CalNotify.Events;
using CalNotify.Models.Auth;

namespace Tests.Integration
{
    [Collection("Global Collection")]
    public class UtilitiesTests  : IClassFixture<StartupFixture<Startup>>
    {
        private readonly StartupFixture<Startup> _fixture;
        public UtilitiesTests(StartupFixture<Startup> fixture)
        {
            _fixture = fixture;

        }

        [Fact]
        public async Task CheckIfAdminCanLogin()
        {
            var loginEvent = new AdminLoginEvent()
            {
                Email = Constants.Testing.TestAdmins.First().Email,
                Password = Constants.Testing.DefaultAdminPass
            };
            var resRefreshToken = await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.AdminConfigurationEndpoint}/login", loginEvent);
            Assert.Equal(200, resRefreshToken.Meta.Code);
            Assert.NotNull(resRefreshToken.Result);
        }   
      
    }

    
}
