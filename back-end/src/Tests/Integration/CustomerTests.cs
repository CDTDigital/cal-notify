using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalNotifyApi;
using CalNotifyApi.Events;
using Xunit;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Models.Responses;
using CalNotifyApi.Services;
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
            using (var context = new BusinessDbContext(_fixture.ContextOptions))
            {
                var userDisabled = context.AllUsers.FirstOrDefault(u => u.Email == temp.Email);
                Assert.NotNull(userDisabled);
                Assert.False(userDisabled.Enabled);
            }
           
            

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


         
            var resResirectToPassword = await _fixture.Get($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);
            Assert.Equal(302, (int)resResirectToPassword.StatusCode);
            var queryPassParam = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                      {
                             new KeyValuePair<string, string>("token", _fixture.FakeToken),
                            new KeyValuePair<string, string>("password", Constants.Testing.DefaultUserPass),
                      });

            var resValidate = await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/password", queryPassParam);

            // Assert
            Assert.Equal(200, resValidate.Meta.Code);
           _fixture.AssertTokenInfo(resValidate.Result);
            using (var context = new BusinessDbContext(_fixture.ContextOptions))
            {
                var user = context.AllUsers.FirstOrDefault(u => u.Email == temp.Email);
                Assert.NotNull(user);
                Assert.True(user.Enabled);
            }

        }

        [Fact, TestPriority(1)]
        public async Task CheckGettingTokenFoExisitingCustomer()
        {
            // Call just in case of test called alone
            var temp = Constants.Testing.TempUsers[1];
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            var resResirectToPassword = await _fixture.Get($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);
            Assert.Equal(302, (int)resResirectToPassword.StatusCode);

            var queryPassParam = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                      {
                             new KeyValuePair<string, string>("token", _fixture.FakeToken),
                            new KeyValuePair<string, string>("password", Constants.Testing.DefaultUserPass),
                      });

            await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/password", queryPassParam);
            var token = 
            await _fixture.Get<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);

            Assert.NotNull(token.Result);
            Assert.NotNull(token.Result.Token);
            Assert.True(token.Result.UserId != Guid.Empty.ToString());

            // TODO: Invalidate token and refresh via reloging in
        }


        [Fact, TestPriority(2)]
        public async Task CheckLogingInUser()
        {
            // Call just in case of test called alone
            var temp = Constants.Testing.TempUsers.First();
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            // validate just in case
            var resResirectToPassword = await _fixture.Get($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);
          

            var queryPassParam = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                      {
                             new KeyValuePair<string, string>("token", _fixture.FakeToken),
                            new KeyValuePair<string, string>("password", Constants.Testing.DefaultUserPass),
                      });

            // set the password just in case
            await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/password", queryPassParam);

            var modelEvent = new UserLoginEvent() {ContactInfo =  Constants.Testing.TempUsers.First().Email, Password = Constants.Testing.DefaultUserPass};
           var token =  await _fixture.Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/login", modelEvent);
            

            Assert.NotNull(token.Result);
            Assert.NotNull(token.Result.Token);
            Assert.True(token.Result.UserId != Guid.Empty.ToString());
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
            var temp = Constants.Testing.TempUsers[1];
            var json = await _fixture.Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/create", temp);

            var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>("token", _fixture.FakeToken),
                       });


            await _fixture.Get($"{Constants.V1Prefix}/{Constants.GenericUserEndpoint}/validate", queryParams);
            

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

