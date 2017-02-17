using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using CalNotify;
using CalNotify.Models.Auth;
using CalNotify.Models.Responses;
using CalNotify.Models.User;
using Tests.Utils;

namespace Tests.Integration
{
    [Collection(TestCollection.Name)]
    [TestCaseOrderer("Tests.Utils.PriorityOrderer", "Tests")]
    public class CustomerTests : IClassFixture<StartupFixture<Startup>>
    {

        private readonly StartupFixture<Startup> _fixture;



        public CustomerTests(StartupFixture<Startup> fixture)
        {
            _fixture = fixture;
           
        }


        [Fact, TestPriority(0)]
        public async Task CheckCreatingCustomer()
        {


            var temp = Constants.Testing.TestUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create",temp);


                
            Assert.Equal(200, json.Meta.Code);
            Assert.NotNull(json.Result);

            // Act 2
            var checkToken = new TokenCheck()
            {
                PhoneNumber = temp.PhoneNumber,
                Token = _fixture.FakeToken
            };
            var resValidate = await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", checkToken);

            // Assert
            Assert.Equal(200, resValidate.Meta.Code);
           _fixture.AssertTokenInfo(resValidate.Result);
        }


        [Fact, TestPriority(1)]
        public async Task CheckGettingTokenForCustomer()
        {
           

            var refreshToken = new RefreshTempUser()
            {
                PhoneNumber = Constants.Testing.TestUsers.First().PhoneNumber
            };

            var resValidate = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.TokenEndpoint}/refresh", refreshToken);
            // Assert
            Assert.Equal(200, resValidate.Meta.Code);
            Assert.NotNull(resValidate.Result);

            var checkToken = new TokenCheck()
            {
                PhoneNumber = Constants.Testing.TestUsers.First().PhoneNumber,
                Token = _fixture.FakeToken
            };

            var resRefreshToken = await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.TokenEndpoint}/validate", checkToken);

            // Assert
            Assert.Equal(200, resRefreshToken.Meta.Code);
            _fixture.AssertTokenInfo(resRefreshToken.Result);
        }


        [Fact, TestPriority(2)]
        public async Task CheckAuthOnResources()
        {
           
            var resUnauth = await _fixture.Get<string>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/");
            // Should hit as an unauthenticated user since we didnt pass our token
            Assert.Equal(401, resUnauth.Meta.Code);
            Assert.Equal(Constants.Messages.UnauthorizedMsg, resUnauth.Meta.Message);
          
        }

        [Fact, TestPriority(3)]
        public async Task CheckGettingSingleCustomer()
        {

            var token = await _fixture.GetToken(Constants.Testing.TestUsers.First());
            var res = await _fixture.Get<GenericUser>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/{token.UserId}", token);
            // Make suer we get back our customer and their properties
            Assert.Equal(200, res.Meta.Code);
            Assert.NotNull(res.Result);
            Assert.NotNull(res.Result);
            Assert.Equal(token.UserId, res.Result.Id.ToString());
        }


        [Fact, TestPriority(4)]
        public async Task CheckGettingAllCustomers()
        {
            var token = await _fixture.GetToken(Constants.Testing.TestUsers.First());
            var res = await _fixture.Get<List<GenericUser>>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}", token);
            Assert.Equal(200, res.Meta.Code);
            Assert.NotNull(res.Result);
            Assert.True(res.Result.Count >=1 );
            var foundCustomer = res.Result.FirstOrDefault(u => u.Name == Constants.Testing.TestUsers.First().Name);
            //Make sure we have our customer in there
            Assert.NotNull(foundCustomer);
        }


    }
}

