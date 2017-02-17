using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CalNotify.Events.Attributes;
using CalNotify.Events.Exceptions;
using CalNotify.Models.Auth;
using CalNotify.Models.Responses;
using CalNotify.Models.Services;
using CalNotify.Services;
using CalNotify.Utils;
using CalNotify.Utils.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Filters;

using Swashbuckle.AspNetCore.Swagger;

namespace CalNotify
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnv;

        private const string TokenAudience = "CalNotifyAudience";
        private const string TokenIssuer = "CalNotifyIssuer";

        internal const string AssemblyName = "CalNotifyApi";


        private RsaSecurityKey _key;
        private TokenAuthOptions _tokenOptions;


        public IConfigurationRoot Configuration { get; }

        internal ExternalServicesConfig ExternalServicesConfig { get; }


        /// <summary>
        /// Inits our Configuration and Loggers
        /// Depending on the environment it may load up different configurations which affect api keys, logging, etc.
        /// The environment is typically set to one of Development, Staging, or Production
        /// </summary>
        /// <param name="env"></param>
        public Startup(IHostingEnvironment env)
        {
            // save our environemnt for later
            _hostingEnv = env;

            // Setting up our configuration
          
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(path: $"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddJsonFile(path: "appsettings.Local.json", optional: false, reloadOnChange: true);
            //.AddEnvironmentVariables();  // not adding environment variables for now

            Configuration = builder.Build();

            // use poco for config
            ExternalServicesConfig = new ExternalServicesConfig();
            Configuration.GetSection("Services").Bind(ExternalServicesConfig);
            

            var loggerConfig = new LoggerConfiguration();


            loggerConfig
                .Enrich.FromLogContext().Enrich.WithProperty("SourceContext", AssemblyName)
                // Application level events
                .WriteTo.Logger(lc => lc
                                    .Filter.ByIncludingOnly(Matching.FromSource("AssemblyName"))
                                    .WriteTo.Console().WriteTo.RollingFile("logs/api-{Date}.txt"))
                // DB logging
                .WriteTo.Logger(lc => lc
                                    .Filter.ByIncludingOnly(Matching.FromSource("Microsoft.EntityFramework"))
                                    .WriteTo.RollingFile("logs/db-{Date}.txt"))
                // All events
                .WriteTo.RollingFile("logs/all-{Date}.txt");
        

            Log.Logger = loggerConfig.CreateLogger();

            Log.Information("Our Content Root is {ContentRoot}", _hostingEnv.ContentRootPath);
            Log.Information("Running server under a {env} environment", env.EnvironmentName);

        }



        public virtual string GetConnectionString()
        {
       
            return Configuration.GetConnectionString(_hostingEnv.EnvironmentName);      
        }


        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                var mvcBuilder = services.AddMvc(
                    options =>
                    {
                        options.Filters.Add(new ValidateModelAttribute());
                        // Add a globally aviable filter for exceptions
                        options.Filters.Add(new EventExceptionFilter());
                    });                              // Add framework services.
              
                ConfigureDB(services);          // We need a persitant storage laye
                ConfigurePolicies(services);    // Our authorization requrires policies to be set
                services.AddTransient<ValidationSender>();

                ConfigureConfigRoot(services);  // Global configuration

                services.AddTransient<GeocodeIO>();
                ConfigureMemoryCaches(services); // short lived, memory based persistance
                ConfigureTokens(services);      // jwt service provider
                ConfigureSwagger(services);
               
               
             

                services.AddCors(); // Register that we want to allow Cross origin sharing of our api resources          

                var defaultJson = Constants.CreateJsonSerializerSettings();


                mvcBuilder.AddJsonOptions(options =>
                {

                    options.SerializerSettings.ReferenceLoopHandling =
                        defaultJson.ReferenceLoopHandling;
                    options.SerializerSettings.NullValueHandling = defaultJson.NullValueHandling;
                    options.SerializerSettings.Converters = defaultJson.Converters;
                    // use standard name conversion of properties
                    options.SerializerSettings.ContractResolver = defaultJson.ContractResolver;
                    options.SerializerSettings.Formatting = defaultJson.Formatting;
                });


                services.AddLogging();


                services.TryAddEnumerable(
                    ServiceDescriptor.Transient<IApiDescriptionProvider, LowerCaseQueryParametersApiDescriptionProvider>
                        ());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "We could not configure services properly");

                throw;
            }
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public  void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            // utilize serilog structured formatting as defined by Log.Logger
            loggerFactory.AddSerilog();

            app.UseMiddleware<SerilogMiddleware>();

            // Add JWT and authorization based security on our api
            AppConfigureJWT(app);

            app.UseStaticFiles();

            // .net core MVC pipeline for routing and other goodies
            app.UseMvc();

            // Utilize swagger if we want awesome api docs
            if (_hostingEnv.IsDevelopment() || _hostingEnv.IsEnvironment("Staging"))
            {

                ConfigureSagger(app);
            }

            // Expose our endpoints to clients across domains
            // TODO: [SECURITY] check if can be removed
            app.UseCors(builder =>
            {
                // Should only be used during development
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
            });


            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BusinessDbContext>();
                // Our connection may be up, lets do some smart connecting to make sure its available.
                TryDBConnect(dbContext);
                dbContext.Database.Migrate();
                // Setup our database with what we need to support our clients
                dbContext.SeedDatabase(_hostingEnv.ContentRootPath, _hostingEnv.IsDevelopment() || _hostingEnv.IsEnvironment("Testing"),app.ApplicationServices).Wait();
                
                dbContext.SaveChanges();

            }

        }

        #region serviceconfiguration

        public virtual void ConfigureDB(IServiceCollection services)
        {
            //Use a PostgreSQL database
            var sqlConnectionString = GetConnectionString();

            services.AddDbContext<BusinessDbContext>(options =>
            {
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly(AssemblyName)
                );
            });
        }

     

     

   

        public virtual void ConfigurePolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.AuthPolicy, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
                options.AddPolicy(Constants.AdminRole, policy => policy.RequireClaim(Constants.RoleClaimKey, Constants.AdminRole));
                options.AddPolicy(Constants.UserRole, policy => policy.RequireClaim(Constants.RoleClaimKey, Constants.UserRole));
                options.AddPolicy(Constants.TechRole, policy => policy.RequireClaim(Constants.RoleClaimKey, Constants.TechRole));
            });
        }



        public virtual void ConfigureMemoryCaches(IServiceCollection services)
        {
            services.AddSingleton<ITokenMemoryCache>(new TokenMemoryCache());
        }

        public virtual void ConfigureConfigRoot(IServiceCollection services)
        {
            // Adds services required for using options.
            services.AddSingleton<ExternalServicesConfig>(ExternalServicesConfig);
        }

        public virtual void ConfigureTokens(IServiceCollection services)
        {
            var contentRootPath = _hostingEnv.ContentRootPath;

            var keyFile = new FileInfo(Path.Combine(contentRootPath, "key.json"));
            RSAParameters keyParams;
            if (keyFile.Exists)
            {
                keyParams = RSAKeyUtils.GetKeyParameters(Path.Combine(contentRootPath, "key.json"));
            }
            else
            {
                RSAKeyUtils.GenerateKeyAndSave(Path.Combine(contentRootPath, "key.json"));
                keyParams = RSAKeyUtils.GetKeyParameters(Path.Combine(contentRootPath, "key.json"));
            }
            

            // Create the key, and a set of token options to record signing credentials 
            // using that key, along with the other parameters we will need in the 
            // token controlller.
            _key = new RsaSecurityKey(keyParams);
            _tokenOptions = new TokenAuthOptions
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                Key = _key,
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.RsaSha256Signature)
            };
            // Save the token options into an instance so they're accessible to the 
            // controller.
            services.AddSingleton(_tokenOptions);
            services.AddTransient<TokenService>();
        }

     
       


    

        #endregion

        #region swagger

        public virtual void ConfigureSwagger(IServiceCollection services)
        {
            if (_hostingEnv.IsDevelopment() || _hostingEnv.IsEnvironment("Staging"))
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info
                    {
                        Version = "v1",
                        Title = "CalNotify API",
                        Description = File.ReadAllText(Path.Combine("Knowledge", "Description.md"))
                    });

                    options.AddSecurityDefinition("token", new ApiKeyScheme
                    {
                        Description =
                            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });

                    options.AddSecurityDefinition("query_token", new ApiKeyScheme()
                    {
                        Description =
                            "JWT token using a query string param. Example: \"? " + Constants.AuthQueryParam + "={token}",
                        Name = Constants.AuthQueryParam,
                        In = "query",
                        Type = "apiKey"
                    });

                    options.OperationFilter<AuthorizationParameterFilter>();
                    options.OperationFilter<AddFileUploadParams>();
                    options.OperationFilter<ModelValidationParameterFilter>();
                    options.OperationFilter<ImagePreviewFilter>();
                });

                services.ConfigureSwaggerGen(options =>
                {
                    //Determine base path for the application.
                    var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                    //Set the comments path for the swagger json and ui.
                    var xmlPath = Path.Combine(basePath, "CalNotifyApi.xml");
                    options.IncludeXmlComments(xmlPath);
                });
            }
        }

        public virtual void ConfigureSagger(IApplicationBuilder app)
        {
            // app.UseStaticFiles();
            app.UseSwagger(c =>
            {
                //c.RouteTemplate = "api-docs/{documentName}/swagger.json";
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUi(c =>
            {
                c.InjectStylesheet("/swagger/theme-feeling-blue.css");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CalNotify API V1");
            });


            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Knowledge")),
                RequestPath = new PathString("/swagger")
            });
        }

        #endregion

        #region appconfigure

        private void AppConfigureJWT(IApplicationBuilder app)
        {
            // Allows us to pass token through a query param instead of through a header of authorization
            app.Use(async (context, next) =>
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                {
                    if (context.Request.QueryString.HasValue)
                    {
                        var token = context.Request.QueryString.Value
                            .Split('&')
                        .SingleOrDefault(x => x.Contains(Constants.AuthQueryParam))?.Split('=')[1];

                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                }
                await next.Invoke();
            });
            // Note, it is VITAL that this is added BEFORE app.UseMvc() is called.
            // See https://github.com/mrsheepuk/ASPNETSelfCreatedTokenAuthExample/issues/11    
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Events = new JwtBearerEvents
                {

                    OnChallenge = async context =>
                    {

                        // Override the response status code.
                        context.Response.StatusCode = Constants.StatusCodes.AuthorizationErrorStatusCode;

                        // Emit the WWW-Authenticate header.
                        context.Response.Headers.Append(
                            HeaderNames.WWWAuthenticate,
                            context.Options.Challenge);

                        await context.Response.WriteAsync(ResponseShell.AuthErrorString(new Meta
                        {
                            Message = Constants.Messages.UnauthorizedMsg,
                            Code = Constants.StatusCodes.AuthorizationErrorStatusCode
                        }));

                        Log.Logger.Warning("Unauthorized access {RemoteIpAddress}", context.HttpContext.Connection);
                        context.HandleResponse();
                    },

                },
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = _key,
                    ValidAudience = _tokenOptions.Audience,
                    ValidIssuer = _tokenOptions.Issuer,

                    // When receiving a token, check that it is still valid.
                    ValidateLifetime = true,

                    // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                    // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                    // machines which should have synchronised time, this can be set to zero. Where external tokens are
                    // used, some leeway here could be useful.
                    ClockSkew = TimeSpan.FromMinutes(0)
                }
            });
        }

        #endregion

       
        private void TryDBConnect(BusinessDbContext dbContext)
        {
            const int NumberOfRetries = 4;
            var retryCount = NumberOfRetries;
            var success = false;
            while (!success && retryCount > 0)
                try
                {
                    dbContext.Database.OpenConnection();
                    success = true;
                }
                catch (Exception ex)
                {
                    retryCount--;
                    var waitSeconds = 3000 * retryCount;
                    Log.Warning(ex, "Waiting for postgres and will retry later {WaitTime:000}", waitSeconds);

                    Task.WaitAll(Task.Delay(waitSeconds));
                    if (retryCount == 0)
                        throw; //or handle error and break/return
                }
        }

       


    }
}

