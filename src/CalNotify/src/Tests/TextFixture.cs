using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CalNotify;
using CalNotify.Events;
using CalNotify.Models.Auth;
using CalNotify.Models.Interfaces;
using CalNotify.Models.Responses;
using CalNotify.Models.User;
using CalNotify.Services;
using CalNotify.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Tests.Utils;
using Xunit;

namespace Tests
{
    public class StartupFixture<TStartup> : BaseFixture, IDisposable where TStartup : class
    {

        private static  TestServer _server;
        public static bool DidClear = false;

        public  string FakeToken = Constants.Testing.TestValidationToken;

        public StartupFixture()
        {

            if (!DidClear)
            {
                DidClear = true;

                //Use a PostgreSQL database
                var sqlConnectionString = Configuration.GetConnectionString("Testing");

                var options = new DbContextOptionsBuilder<BusinessDbContext>();
                options.UseNpgsql(sqlConnectionString, b => b.MigrationsAssembly(Constants.ProjectName)
                );
                using (var dbContext = new BusinessDbContext(options.Options))
                {
                    dbContext.Database.OpenConnection();
                    dbContext.AllUsers.Clear();
                    dbContext.SaveChanges();

                }
            }

            if (_server == null)
            {
                var solutionRelativeTargetProjectParentDir = Path.Combine("src");
                var contentRoot = CalNotify.Utils.Extensions.GetProjectPath(solutionRelativeTargetProjectParentDir);
                var builder = new WebHostBuilder()
                    .UseContentRoot(contentRoot)
                    .UseEnvironment("Testing")
                    .UseStartup<TStartup>().ConfigureServices(InitializeServices);

               
                _server = new TestServer(builder);

     
            }
         
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
           var  mockedSmsSender = new Mock<ISmsSender>();
            mockedSmsSender.Setup(x =>
                x.SendValidationToken(It.IsAny<ITokenAble>())).ReturnsAsync(FakeToken);
            services.Remove(new ServiceDescriptor(typeof(ISmsSender), typeof(TwilioSmsSender)));
            services.AddSingleton(provider => mockedSmsSender.Object);
        }

        public new void Dispose()
        {
         
           
        }



        #region publichelpers
        public async Task<ResponseShell<T>> Post<T>(string route, object param, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }

                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                     await (await client.PostAsync(route, new JsonContent(param))).Content
                         .ReadAsStringAsync();


                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }

        }

        public async Task<ResponseShell<T>> Put<T>(string route, object param, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }

                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.PutAsync(route, new JsonContent(param))).Content
                        .ReadAsStringAsync();


                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }
        }



        public async Task<ResponseShell<T>> PostForm<T>(string route, MultipartFormDataContent content,
            TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }

                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.PostAsync(route, content)).Content
                        .ReadAsStringAsync();

                client.Dispose();
                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }
        }

        public async Task<ResponseShell<T>> Put<T>(string route, MultipartFormDataContent data,
         TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                string query = "";
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>(Constants.AuthQueryParam, tokenInfo.Token),
                       });

                    query = "?" + queryParams.ReadAsStringAsync().Result;
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }

                client.BaseAddress = new Uri("http://localhost");
                var response = await client.PutAsync(route + query, data);
                var responseValidate = await response.Content.ReadAsStringAsync();


                client.Dispose();
                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }

        }
        public async Task<ResponseShell<T>> Get<T>(string route, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                string query = "";
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>(Constants.AuthQueryParam, tokenInfo.Token),
                       });

                    query = "?" + queryParams.ReadAsStringAsync().Result;
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }
                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.GetAsync(route + query)).Content
                        .ReadAsStringAsync();
                client.Dispose();
                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }
        }


        public async Task<ResponseShell<T>> Get<T>(string route, FormUrlEncodedContent queryParams, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                string query = "";
                if (!Equals(tokenInfo, default(TokenInfo)))
                {

                    query = "?" + queryParams.ReadAsStringAsync().Result;
                   
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.GetAsync(route + query)).Content
                        .ReadAsStringAsync();
                client.Dispose();
                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }
        }

        public async Task<FileContentResult> Get(string route, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                string query = "";
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>(Constants.AuthQueryParam, tokenInfo.Token),
                       });

                    query = "?" + queryParams.ReadAsStringAsync().Result;
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }
                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.GetAsync(route + query)).Content
                        .ReadAsByteArrayAsync();
                client.Dispose();
                return new FileContentResult(responseValidate, "image/png");
            }
        }

        public async Task<string> GetBase64(string route, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                string query = "";
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    var queryParams = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                       {
                            new KeyValuePair<string, string>(Constants.AuthQueryParam, tokenInfo.Token),
                       });

                    query = "?" + queryParams.ReadAsStringAsync().Result;
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }
                client.BaseAddress = new Uri("http://localhost");

                var responseValidate =
                    await (await client.GetAsync(route + query)).Content
                        .ReadAsStringAsync();
                client.Dispose();
                return responseValidate;
            }
        }

        public async Task<ResponseShell<T>> Delete<T>(string route, TokenInfo tokenInfo = default(TokenInfo))
        {
            using (var client = _server.CreateClient())
            {
                if (!Equals(tokenInfo, default(TokenInfo)))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.Token);
                }
                client.BaseAddress = new Uri("http://localhost");
                var responseValidate =
                    await (await client.DeleteAsync(route)).Content
                        .ReadAsStringAsync();
                client.Dispose();
                return JsonConvert.DeserializeObject<ResponseShell<T>>(responseValidate);
            }
        }

     

     

      
        #endregion





        public async Task<TokenInfo> GetToken(GenericUser user)
        {
            var refreshToken = new TempUserRefreshTempUserSmsWithSms()
            {
                PhoneNumber = user.PhoneNumber
            };
            var resValidate = await Post<SimpleSuccess>($"{Constants.V1Prefix}/{Constants.TokenEndpoint}/refresh", refreshToken);
            if (resValidate.Result == null || resValidate.Result.Success == false)
            {
                throw new Exception(resValidate.Meta.Message);
            }
            var checkToken = new TokenCheck()
            {
                PhoneNumber = user.PhoneNumber,
                Token = FakeToken
            };

            var resRefreshToken = await Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.TokenEndpoint}/validate", checkToken);
            if (resRefreshToken.Result == null)
            {
                throw new Exception(resValidate.Meta.Message);
            }
            return resRefreshToken.Result;
        }


        public async Task<TokenInfo> AdminLogin()
        {

            var loginEvent = new AdminLoginEvent()
            {
                Email = Constants.Testing.TestAdmins.First().Email,
                Password = Constants.Testing.DefaultAdminPass
            };
            var resRefreshToken = await Post<TokenInfo>($"{Constants.V1Prefix}/{Constants.AdminConfigurationEndpoint}/login", loginEvent);
            if (resRefreshToken.Result == null)
            {
                throw new Exception(resRefreshToken.Meta.Message);
            }
            return resRefreshToken.Result;
        }




        public void AssertTokenInfo(TokenInfo token)
        {

            // Must have a result
            Assert.NotNull(token);
            // Must give back the user id for subsequent calls to the api
            Assert.True(!string.IsNullOrEmpty(token.UserId));
            //we need an api token to utilize with the rest of the api
            Assert.NotNull(token.Token);
            // The clients need to know when their token will expire
            Assert.NotNull(token.Expires);

        }
    }
}
