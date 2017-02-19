using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalNotifyApi;
using Xunit;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
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


            var temp = Constants.Testing.TempUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create",temp);


                
            Assert.Equal(200, json.Meta.Code);
            Assert.NotNull(json.Result);

      
            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            var resValidate = await _fixture.Get<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);

            // Assert
            Assert.Equal(200, resValidate.Meta.Code);
           _fixture.AssertTokenInfo(resValidate.Result);
        }


        [Fact, TestPriority(1)]
        public async Task CheckGettingTokenForCustomer()
        {
            // Call just in case of test called alone
            var temp = Constants.Testing.TempUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


             await _fixture.Get<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);


            var refreshToken = new RefreshTempUser()
            {
                PhoneNumber = Constants.Testing.TempUsers.First().PhoneNumber
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
            // Call just in case of test called alone
            var temp = Constants.Testing.TempUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            var resValidate = await _fixture.Get<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);


            var token = await _fixture.AdminLogin();
            var res = await _fixture.Get<GenericUser>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/{resValidate.Result.UserId}", token);
            // Make suer we get back our customer and their properties
            Assert.Equal(200, res.Meta.Code);
            Assert.NotNull(res.Result);
            Assert.NotNull(res.Result);
            Assert.Equal(resValidate.Result.UserId, res.Result.Id.ToString());
        }

       
        [Fact, TestPriority(4)]
        public async Task CheckGettingAllCustomers()
        {

            // Call just in case of test called alone
            var temp = Constants.Testing.TempUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            var resValidate = await _fixture.Get<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);


            var token = await _fixture.AdminLogin();
            var res = await _fixture.Get<List<GenericUser>>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}", token);
            Assert.Equal(200, res.Meta.Code);
            Assert.NotNull(res.Result);
            Assert.True(res.Result.Count >=1 );
            var foundCustomer = res.Result.FirstOrDefault(u => u.Name == Constants.Testing.TempUsers.First().Name);
            //Make sure we have our customer in there
            Assert.NotNull(foundCustomer);
        }


    }
}

