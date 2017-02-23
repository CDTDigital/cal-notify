using System.Collections.Generic;
using System.Linq;
using CalNotifyApi.Events;
using CalNotifyApi.Models;
using CalNotifyApi.Models.Addresses;
using CalNotifyApi.Models.Auth;
using CalNotifyApi.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NpgsqlTypes;

namespace CalNotifyApi
{
    /// <summary>
    /// All of our static or constant variables which are used throughout the api
    /// </summary>
    public static class Constants
    {

        public const string SolutionName = "CalNotify.sln";
        public const string ProjectName = "CalNotifyApi";

        #region routing
        /// <summary>
        /// The version modifier for all of the endpoints
        /// </summary>
        public const string V1Prefix = "/v1";

        /// <summary>
        /// Prefix used for accessing customer related resources and actions
        /// </summary>
        public const string GenericUserEndpoint = "users";


        /// <summary>
        /// Prefix used for accessing token related resources and actions.
        /// </summary>
        public const string TokenEndpoint = "tokens";


        /// <summary>
        /// Prefix used for utilities and other related endpoints
        /// </summary>
        public const string UtilsEndpoint = "utils";

        /// <summary>
        /// Prefix used for administrative and other configuration  related endpoints
        /// </summary>
        public const string AdminConfigurationEndpoint = "admin";


        public const string ValidationAction = "validate";


        #endregion


        #region Security

        /// <summary>
        /// The generic authorization policy which all locked down endpoints should have
        /// </summary>
        public const string AuthPolicy = "Bearer";

        /// <summary>
        /// Role a user is in if an admin.
        /// </summary>
        public const string AdminRole = "Admin";

        /// <summary>
        /// Role a user is in if a customer
        /// </summary>
        public const string UserRole = "User";

        /// <summary>
        /// Role a user is in if a technician
        /// </summary>
        public const string TechRole = "Tech";


       

        /// <summary>
        /// Complete list of roles used in the system
        /// </summary>
        public static string[] SystemRoles = new[] { AdminRole, UserRole, TechRole };

        /// <summary>
        /// The key set in a user's jwt with a value of their user id
        /// </summary>
        public const string UserIdClaimKey = "UserId";

        /// <summary>
        /// The key to set for role based claims in the firebase auth token
        /// </summary>
        public const string RoleClaimKey = "Role";

        /// <summary>
        /// The maximum total number of days which a jwt security token is valid
        /// </summary>
        public const int ExpirationOffsetInDays = 90;

        /// <summary>
        /// The query param to pass if clients do not wish to use a header value of Authorization
        /// </summary>
        public const string AuthQueryParam = "auth_token";


        #endregion

        public static class PropetyNames
        {
            public const string UserId = "id";
            public const string AdminId = "admin_id";
        }




        public static class Messages
        {
          

          

            public const string EmailValidationFailure = "Email validation is unable to function properly. Check the conifguration and make sure the values are properly set";

            /// <summary>
            /// The message returned when an unauthenticated request hits our non-anonyomous endpoints
            /// </summary>
            public const string UnauthorizedMsg = "Authentication failed. The request must include a valid and non-expired bearer token in the Authorization header.";

            /// <summary>
            /// The message returned when the server encountered an unrecoverable error
            /// </summary>
            public const string UnexpectedErrorMsg = "Server side error.";

            /// <summary>
            /// The message sent out when a user has authenticated, but their token or claims is not sufficent to acces the endpoint or the resource.
            /// </summary>
            public const string InvalidClaimMsg =
                "Forbidden. Your token does not allow you to access this resource. Your token only allows you to modify certain resources.";

            /// <summary>
            /// Generic message when an event model fails due to validation attributes not being satisfied
            /// </summary>
            public const string InvalidModelMsg = "Failure at endpoint due to validation, see meta object for more details";

            /// <summary>
            /// Generic Message to send when a validation for a user id fails.
            /// </summary>
            public const string UserNotFound = "No User found with that it";

         

        }


        public static class StatusCodes
        {
            public const int ModelErrorStatusCode = 400;

            public const int AuthorizationErrorStatusCode = 401;

            public const int EventErrorStatusCode = 400;

            public const int ErrorUnexpcetedStatusCode = 500;
        }

        #region Misc
 
        public  const string ObsoleteOnlyForBinding = "Only for model Binding";

        /// <summary>
        /// The default Spatial reference system used.
        /// </summary>
        public const int SRID = 4326;

        /// <summary>
        /// Default JSON serialization options used via mvc actions
        /// </summary>
        public static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new Newtonsoft.Json.Converters.StringEnumConverter(),
                    new Newtonsoft.Json.Converters.KeyValuePairConverter()
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented

            };
        }

        #endregion

        #region swagger

     
        /// <summary>
        /// Swagger related tag for authorization actions
        /// </summary>
        public const string AuthorizationTag = "authorizations";

        #endregion


        public static class Testing
        {
            public const string TestValidationToken = "1234";

            public const string DefaultUserPass = "123testUser";

            public const string DefaultAdminPass = "123testadmin";

            public const string UserOverride = "testUser";



            public static bool CheckIfOverride(GenericUser user)
            {
                return TestNumbers.Contains(user.PhoneNumber) || user.Name.Contains(UserOverride);

            }
            public static bool CheckIfOverride(TempUser user)
            {
                return TestNumbers.Contains(user.PhoneNumber);

            }


            public static string[] TestNumbers = new[]
            {
                // 0 - 3
                "916-111-0001", "916-111-0002", "916-111-0003", "916-111-0004", // customers
                // 4 - 8
                "916-222-0001", "916-222-0002", "916-222-0003", "916-222-0004", "916-222-0005", // techs

                // 9
                "916-333-0001" // admins
            };


            public static GenericUser[] TestUsers = new GenericUser[]
            {
                new GenericUser()
                {
                    Email = "testUser1@test.com",
                    PhoneNumber = TestNumbers[0],
                    Name = UserOverride + "0",
                    Address =  new Address()
                {
                    City = "Sacramento",
                    State = "California",
                    Street = "Test Street",
                    Zip = "12345",
                    GeoLocation = new PostgisPoint( 38.5816, -121.4944) {SRID = Constants.SRID}

                }
                },
                 new GenericUser()
                {
                    Email = "testUser2@test.com",
                    PhoneNumber = TestNumbers[1],
                    Name = UserOverride + "1"
                },
            };
         
            public static TempUser[] TempUsers = new TempUser[]
            {
                    new TempUser()
                {
                    Email = "testUser3@test.com",
                    PhoneNumber = TestNumbers[2],
                    Name = UserOverride + "3",
                    City ="Antelope",
                    Latitude = 38.5816,
                    Longitude = -121.4944,
                    Number = "5344", 
                    State = "CA",
                    Street = "Test Street",
                    Zip = "95843"

                },
                       new TempUser()
                {
                    Email = "testUser4@test.com",
                    PhoneNumber = TestNumbers[3],
                    Name = UserOverride + "4",
                    City ="Antelope",
                    Latitude = 38.5816,
                    Longitude = -121.4944,
                    Number = "5344",
                    State = "CA",
                    Street = "Test Street",
                    Zip = "95843"

                },
            };

            public static CreateAdminEvent[] TestAdmins = new[]
            {
                new CreateAdminEvent()
                {
                     Email = "testAdmin1@test.com",
                        Name = "testAdmin1",
                        Password = DefaultAdminPass,
                        PhoneNumber = TestNumbers[9]
                }
            };

        

            public static Address[] TestAddressWithLatLngs = new[]
            {
                new Address()
                {
                    City = "Sacramento",
                    State = "California",
                    Street = "Test Street",
                    Zip = "12345",
                    GeoLocation = new PostgisPoint( 38.5816, -121.4944) {SRID = Constants.SRID}
                    
                }
            };

        }

    }
}
