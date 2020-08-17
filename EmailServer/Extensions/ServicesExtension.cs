using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authorization;
// using Utf8Json;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Identity;

namespace EmailServer
{
    /// <summary>
    /// Services extension is a class that extended all the available methods within IServiceCollection interface
    /// wrote by Microsoft. We extended it here to suit our needs with the server configurations and register 
    /// controller into our dependency injection containter.
    /// </summary>
    public static class ServicesExtension
    {
        /// <summary>
        /// Register and initialize database connection for the application
        /// </summary>
        public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // Setup Database Context and register it in DI
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("LocalConnection"));      // Connect to Local Docker SQL sever
            });
            return services;
        }

        /// <summary>
        /// Register and initialize custom MVC
        /// </summary>
        public static IServiceCollection AddCustomMVC(this IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddFormatterMappings()
                .AddDataAnnotations()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
            return services;
        }

        public static IServiceCollection AddCustomBehaviors(this IServiceCollection services)
        {
            // Configure auto model validation error
            services.Configure<ApiBehaviorOptions>(_ConfigApiBehavior);

            return services;
        }

        /// <summary>
        /// Register and initialize repositories objects for dependency injection.!--
        /// 
        /// https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
        /// NOTE + Transient objects are always different; a new instance is provided 
        ///        to every controller and every service.
        ///      + Scoped objects are the same within a request, 
        ///        but different across different requests.
        ///      + Singleton objects are the same for every object and every request.
        /// </summary>
        public static IServiceCollection AddCustomTransientServices(this IServiceCollection services, IConfiguration configuration)
        {
            // SEND GRID

            // Configure services
            services.Configure<SendGridEmailServiceOptions>(configuration.GetSection("EmailSendGrid"));

            // Add transient services
            services.AddTransient<IEmailRepository, EmailRepository>();

            return services;
        }

        /// <summary>
        /// Configurate JWT authentication layer for the web API
        /// TODO Implement the JWT Auth Bearer Token in later update 
        /// </summary>
        public static IServiceCollection AddCustomAuth(this IServiceCollection services, IConfiguration configuration)
        {
            // // Remove default JWT claims
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // // Add JWT Authentication for Api clients
            // services.AddAuthentication(_ConfigAuthentication)
            //     // .AddJwtBearer(_ConfigJwt);
            //     .AddJwtBearer(options =>
            //     {
            //         // Set validation parameters
            //         options.TokenValidationParameters = new TokenValidationParameters()
            //         {
            //             // Validate issuer
            //             ValidateIssuer = true,
            //             // Set issuer
            //             ValidIssuer = configuration["Jwt:Issuer"],

            //             // Validate audience
            //             ValidateAudience = true,
            //             // Set audience
            //             ValidAudience = configuration["Jwt:Audience"],

            //             // Validate expiration
            //             ValidateLifetime = true,
            //             // Remove delay of token when expire (5 min is default & or less is recommened)
            //             ClockSkew = TimeSpan.Zero,

            //             // Validate signature
            //             RequireSignedTokens = true,
            //             ValidateIssuerSigningKey = true,
            //             // Set signing key
            //             IssuerSigningKey = new SymmetricSecurityKey(
            //                 // Get our secret key from configuration
            //                 Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])
            //             ),
            //         };
            //         options.Events = new JwtBearerEvents()
            //         {
            //             // OnAuthenticationFailed = _ConfigFailedJwtResponse
            //             OnChallenge = _ConfigFailedJwtResponse
            //         };
            //     }
            // );

            // Hosting doesn't add IHttpContextAccessor by default
            services.AddHttpContextAccessor();

            // // AddIdentity adds cookie based authentication
            // // Adds scoped classes for things like UserManager, SignInManager, PasswordHashers etc..
            // // NOTE: Automatically adds the validated user from a cookie to the HttpContext.User
            // // NOTE: We will use JWT over cookies, but the HttpContext will still be populated with
            // // our JWT claims
            // // Also, Change password, lockout, & user policy
            // services.AddIdentityCore<UserIdentity>(_ConfigIdentity)

            //     // Adds UserStore and RoleStore from this context
            //     // That are consumed by the UserManager and RoleManager
            //     // https://github.com/aspnet/Identity/blob/dev/src/EF/IdentityEntityFrameworkBuilderExtensions.cs
            //     .AddEntityFrameworkStores<ProjectionDbContext>()

            //     // Adds a provider that generates unique keys and hashes for things like
            //     // forgot password links, phone number verification codes etc...
            //     // Expires in 24 hours
            //     .AddDefaultTokenProviders()

            //     // Adds a provider that generates unique keys and hashes for things like
            //     // verify account, activate account, etc...
            //     // Expires in 7 days
            //     .AddWeekSpanTokenProvider();

            return services;
        }


        /// <summary>
        /// Configurate Controllers for the application
        /// </summary>
        public static IServiceCollection AddCustomControllers(this IServiceCollection services)
        {
            // AddControllersCore
            services
                .AddMvcCore() // TODO: Add Utf8Json later for json parser
                .AddApiExplorer()
                // .AddAuthorization(_ConfigAuthorization) // TODO: Add later policies
                .AddCors()
                .AddDataAnnotations()
                .AddFormatterMappings();
            // .AddFluentValidation(fv =>
            // {
            //     fv.RegisterValidatorsFromAssemblyContaining<Startup>();
            //     fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            // });

            return services;
        }

        /// <summary>
        /// Register all the Respositories or API Services into the application
        /// 
        /// https://stackoverflow.com/questions/38138100/addtransient-addscoped-and-addsingleton-services-differences
        /// NOTE + Transient objects are always different; a new instance is provided 
        ///        to every controller and every service.
        ///      + Scoped objects are the same within a request, 
        ///        but different across different requests.
        ///      + Singleton objects are the same for every object and every request.
        /// </summary>
        // public static IServiceCollection AddCutomScopedServices(this IServiceCollection services, IConfiguration configuration)
        // {
        //     // Config SendGrid API Key for email sending
        //     services.Configure<SendGridEmailServiceOptions>(configuration.GetSection("EmailSendGrid"));
        //     services.AddScoped<IEmailRepository, EmailRepository>();
        //     return services;
        // }


        // #region Service Configs

        // private static void _ConfigAuthentication(AuthenticationOptions options)
        // {
        //     // Setting JWT as our default authentication (Not cookies)
        //     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        // }

        // private static async Task _ConfigFailedJwtResponse(
        //     // AuthenticationFailedContext context
        //     JwtBearerChallengeContext context
        // )
        // {
        //     // TODO: Log exception msg
        //     // TODO: Find out the difference btw OnAuthenticationFailed & OnChallenge

        //     // Init message
        //     string message = "No token";

        //     /**
        //     For OnAuthenticationFailed if we want the exception msg DEGUB ONLY
        //     Console.WriteLine(
        //         String.Join(
        //             ' ',
        //             "-------------------",
        //             $"[ConfigFailedJwtResponse]",
        //             $": context.Exception.Message: {context.Exception.Message}",
        //             "-------------------"
        //         )
        //     );
        //     */
        //     // Extract exception type
        //     // var exType = context.Exception.GetType(); /** For OnAuthenticationFailed */
        //     Type exType = typeof(SecurityTokenException); /** For OnChallenge */
        //     if (context.AuthenticateFailure != null)
        //     {
        //         exType = context.AuthenticateFailure.GetType();
        //     }
        //     // If the authorization header exists
        //     else if (context.Request.Headers.ContainsKey(AuthConst.StrAuthorization))
        //     {
        //         exType = typeof(SecurityTokenValidationException);
        //     }
        //     else
        //     {
        //         // No auth header, no token
        //         // NTD
        //     }

        //     // Get custom message based on exception type
        //     if (exType == typeof(SecurityTokenValidationException))
        //     {
        //         message = "Invalid token";
        //     }
        //     else if (exType == typeof(SecurityTokenInvalidIssuerException))
        //     {
        //         message = "Invalid issuer";
        //     }
        //     else if (exType == typeof(SecurityTokenExpiredException))
        //     {
        //         message = "Expired token";
        //     }
        //     else
        //     {
        //         // NTD
        //     }

        //     // Construct server response
        //     ServerResponseMessageV1 response = new ServerResponseMessageV1(false, message)
        //     {
        //         Code = ErrorCode.Unauthorized
        //     };

        //     // context.NoResult(); /** For OnAuthenticationFailed */
        //     context.HandleResponse(); /** For OnChallenge */

        //     // Set return type to JSON
        //     context.Response.ContentType = $"{HttpConst.ContentTypeJson}; {HttpConst.CharsetUtf8}";

        //     // Set status to unauthorized
        //     context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        //     // SPA framework very strict on response header. Ignores data if these are missing
        //     context.Response.Headers.Add(HttpConst.CorsOrigin, new[] { HttpConst.AcceptedOrigin });
        //     context.Response.Headers.Add(HttpConst.CorsHeaders, new[] { HttpConst.AcceptedHeaders });
        //     context.Response.Headers.Add(HttpConst.CorsMethods, new[] { HttpConst.AcceptedMethods });
        //     context.Response.Headers.Add(HttpConst.CorsCredentials, new[] { Boolean.TrueString });

        //     // Return response
        //     await context.Response.WriteAsync(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase), Encoding.UTF8);
        // }

        // private static void _ConfigAuthorization(AuthorizationOptions options)
        // {
        //     options.AddPolicy(Policies.GlobalAdminsOnly, policy => policy.RequireRole(OrgType.Sudoer.ToValue()));
        //     options.AddPolicy(Policies.IsConfirmed, policy => policy.RequireClaim(AuthConst.JwtConfirmed, CommonConst.StrY));
        // }

        // private static void _ConfigIdentity(IdentityOptions options)
        // {
        //     // Make really weak passwords possible
        //     options.Password.RequiredLength = 8;
        //     options.Password.RequireDigit = false;
        //     options.Password.RequireLowercase = true;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequireNonAlphanumeric = false;

        //     // Make sure users have unique emails
        //     options.User.RequireUniqueEmail = false;
        //     options.Lockout.MaxFailedAccessAttempts = 50;
        // }

        private static void _ConfigApiBehavior(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = (ActionContext context) =>
            {
                ServerResponseValidationError response = new ServerResponseValidationError(context.ModelState);
                BadRequestObjectResult result = new BadRequestObjectResult(response);

                result.ContentTypes.Add(HttpConst.ContentTypeJson);
                result.ContentTypes.Add(HttpConst.ContentTypeXml);

                return result;
            };
            options.SuppressModelStateInvalidFilter = true;
        }

        // private static void _ConfigEventStoreEventHandlers(IEventStoreConnection connection, ILogger logger)
        // {
        //     // Setup critical handlers (Halts the app)
        //     connection.ErrorOccurred += (sender, args) =>
        //     {
        //         // throw new Exception($"Event Store Connection Failed :: {args.Exception.Message}", args.Exception);
        //         logger.LogCritical("---------- Event Store Connection Failed :: {reason} ----------", args.Exception.Message);
        //         Program.Shutdown();
        //     };
        //     connection.Closed += (sender, args) =>
        //     {
        //         // https://github.com/EventStore/EventStore/blob/master/src/EventStore.ClientAPI/Internal/EventStoreConnectionLogicHandler.cs
        //         switch (args.Reason)
        //         {
        //             case "Connection close requested by client.":
        //                 logger.LogTrace("Connection to event store closed because {reason}", args.Reason);
        //                 logger.LogInformation("---------- Event Store Closed! ----------");
        //                 break;

        //             case "Failed to resolve TCP end point to which to connect.":
        //             case "No end point to node specified.":
        //             case "TCP connection error occurred.":
        //             case "Reconnection limit reached.":
        //             case "Connection-wide BadRequest received. Too dangerous to continue.":
        //             case "No end point is specified while trying to reconnect.":
        //             default:
        //                 // throw new Exception($"Event Store Connection Closed :: {args.Reason}");
        //                 logger.LogCritical("---------- Event Store Closed :: {reason} ----------", args.Reason);
        //                 Program.Shutdown();
        //                 break;
        //         }
        //     };

        //     // Setup status handlers
        //     connection.Disconnected += (sender, args) => logger.LogTrace("Connection to {endpoint} event store disconnected.", args.RemoteEndPoint);
        //     connection.Reconnecting += (sender, args) => logger.LogTrace("Reconnecting to event store.");
        //     connection.Connected += (sender, args) =>
        //     {
        //         logger.LogTrace("Connection to {endpoint} event store established.", args.RemoteEndPoint);
        //         logger.LogInformation("---------- Event Store Ready! ----------");
        //     };
        // }

        // #endregion
    }
}