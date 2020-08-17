using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Utf8Json;
using Utf8Json.Resolvers;

namespace EmailServer
{

    /// <summary>
    /// This class is an extension for application services. Mainly use for Startup.cs
    /// </summary>
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseExpressPreflight(this IApplicationBuilder app)
        {
            app.Use((context, next) =>
            {
                // Alter the HTTP OPTIONS request created from Localhost or dev env
                if (context.Request.Method == HttpConst.MethodOptions)
                {
                    context.Response.Headers.Add(HttpConst.CorsOrigin, new[] { HttpConst.AcceptedOrigin });
                    context.Response.Headers.Add(HttpConst.CorsHeaders, new[] { HttpConst.AcceptedHeaders });
                    context.Response.Headers.Add(HttpConst.CorsMethods, new[] { HttpConst.AcceptedOrigin });
                    context.Response.Headers.Add(HttpConst.CorsCredentials, new[] { Boolean.TrueString });
                    return context.Response.WriteAsync(Constants.StrOk);
                }

                // Invoke the next method if this isn't HTTP OPTIONS request
                return next.Invoke();
            });

            return app;
        }

        public static IApplicationBuilder UseCustomPrivacy(this IApplicationBuilder app)
        {
            app.Use((context, nextTask) =>
            {
                context.Response.Headers.Add("X-Frame-Options", new[] { "DENY" });
                context.Response.Headers.Add("X-Download-Options", new[] { "noopen" });
                context.Response.Headers.Add("X-Content-Type-Options", new[] { "nosniff" });
                context.Response.Headers.Add("X-Xss-Protection", new[] { "1; mode=block" });
                context.Response.Headers.Add("Referrer-Policy", new[] { "same-origin" });
                return nextTask();
            });
            return app;
        }

        public static IApplicationBuilder UseCustomHTMLPage(this IApplicationBuilder app)
        {
            // Add the index html to the default path of the application
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();
            return app;
        }

        public static IApplicationBuilder UserCustomStatusCodePage(this IApplicationBuilder app)
        {
            return app.UseStatusCodePages(_HandleStatusCodeResponse);
        }

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(_HandleException);
            return app;
        }

        private static async Task _HandleStatusCodeResponse(StatusCodeContext handler)
        {
            string errorMsg = ErrorCodes.ServerError;
            switch (handler.HttpContext.Response.StatusCode)
            {
                case StatusCodes.Status400BadRequest:
                    {
                        errorMsg = ErrorCodes.BadRequest;
                        break;
                    }

                case StatusCodes.Status401Unauthorized:
                    {
                        errorMsg = ErrorCodes.Unauthorized;
                        break;
                    }

                case StatusCodes.Status403Forbidden:
                    {
                        errorMsg = ErrorCodes.Forbidden;
                        break;
                    }

                case StatusCodes.Status404NotFound:
                    {
                        // handler.HttpContext.Request.Path (if we want to include path)
                        errorMsg = ErrorCodes.NotFound;
                        break;
                    }

                case StatusCodes.Status422UnprocessableEntity:
                    {
                        errorMsg = ErrorCodes.UnprocessableRequest;
                        break;
                    }

                case StatusCodes.Status500InternalServerError:
                default:
                    {
                        // NTD as default string = server_error
                        break;
                    }
            }
            // Construct server response
            ServerResponseBase response = new ServerResponseBase(false)
            {
                Code = errorMsg
            };
            // Set response type to JSON
            handler.HttpContext.Response.ContentType = $"{HttpConst.ContentTypeJson}; {Constants.CharsetUtf8}";
            // SPA framework very strict on response header. Ignores data if these are missing
            handler.HttpContext.Response.Headers.Add(HttpConst.CorsOrigin, new[] { HttpConst.AcceptedOrigin });
            handler.HttpContext.Response.Headers.Add(HttpConst.CorsHeaders, new[] { HttpConst.AcceptedHeaders });
            handler.HttpContext.Response.Headers.Add(HttpConst.CorsMethods, new[] { HttpConst.AcceptedMethods });
            handler.HttpContext.Response.Headers.Add(HttpConst.CorsCredentials, new[] { Boolean.TrueString });
            // Send response
            await handler.HttpContext.Response.WriteAsync(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase), Encoding.UTF8);
        }

        private static void _HandleException(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                // Init message & code
                Object message = "Unhandled exception";
                string responseCode = ErrorCodes.ServerError;
                // Set default status code to server error
                int statusCode = StatusCodes.Status500InternalServerError;
                // Extract exception handler from the OG handler/middleware
                IExceptionHandlerFeature exHandler = context.Features.Get<IExceptionHandlerFeature>();
                if (exHandler != null)
                {
                    // Extract exception
                    var ex = exHandler.Error;
                    // Extract exception type
                    var exType = ex.GetType();
                    // Get custom message based on exception type
                    if (exType == typeof(DbUpdateConcurrencyException))
                    {
                        // Casting the exception to its proper type for data extraction
                        DbUpdateConcurrencyException scopedEx = (ex as DbUpdateConcurrencyException);
                        // Client value
                        var clValue = scopedEx?.Entries.FirstOrDefault().Entity;
                        // Database value
                        var dbValue = (await scopedEx?.Entries.FirstOrDefault().GetDatabaseValuesAsync()).ToObject();
                        message = new { client = clValue, database = dbValue };
                        responseCode = ErrorCodes.EntityChanged;
                        statusCode = StatusCodes.Status409Conflict;
                    }
                    else if (exType == typeof(DbUpdateException))
                    {
                        // Db exceptions seem to have better details in the inner exception
                        message = ex?.InnerException?.Message ?? ex.Message;
                    }
                    else if (exType == typeof(ApiForbiddenException))
                    {
                        // Casting the exception to its proper type for data extraction
                        ApiForbiddenException scopedEx = (ex as ApiForbiddenException);
                        message = scopedEx.Message ?? "Top secret";
                        responseCode = scopedEx.Code ?? ErrorCodes.Forbidden;
                        statusCode = StatusCodes.Status403Forbidden;
                    }
                    else if (exType == typeof(ApiUnauthorizedException))
                    {
                        // Casting the exception to its proper type for data extraction
                        ApiUnauthorizedException scopedEx = (ex as ApiUnauthorizedException);
                        message = scopedEx.Message ?? "Shoo!!! Not allowed";
                        responseCode = scopedEx.Code ?? ErrorCodes.Unauthorized;
                        statusCode = StatusCodes.Status401Unauthorized;
                    }
                    else if (exType == typeof(ApiInvalidModelException))
                    {
                        // Casting the exception to its proper type for data extraction
                        ApiInvalidModelException scopedEx = (ex as ApiInvalidModelException);
                        message = scopedEx.Message ?? "Semi naked entity";
                        responseCode = scopedEx.Code ?? ErrorCodes.BadRequest;
                        statusCode = StatusCodes.Status400BadRequest;
                    }
                    else if (exType == typeof(ApiUnprocessableException))
                    {
                        // TODO: Potentially extract errors out of this exception
                        // Casting the exception to its proper type for data extraction
                        ApiUnprocessableException scopedEx = (ex as ApiUnprocessableException);
                        message = scopedEx.Message ?? "Error completing request";
                        responseCode = scopedEx.Code ?? ErrorCodes.UnprocessableRequest;
                        statusCode = StatusCodes.Status422UnprocessableEntity;
                    }
                    else if (exType == typeof(ApiNotFoundException))
                    {
                        // Casting the exception to its proper type for data extraction
                        ApiNotFoundException scopedEx = (ex as ApiNotFoundException);
                        message = scopedEx.Message ?? "It's gone or never here";
                        responseCode = scopedEx.Code ?? ErrorCodes.NotFound;
                        statusCode = StatusCodes.Status404NotFound;
                    }
                    else if (exType == typeof(KeyNotFoundException))
                    {
                        // Custom exception testing
                        message = ex.Message ?? "I'm blind";
                        responseCode = ErrorCodes.NotFound;
                        statusCode = StatusCodes.Status404NotFound;
                    }
                    else if (exType == typeof(NotImplementedException))
                    {
                        // Custom exception testing
                        message = "Under construction";
                    }
                    else
                    {
                        // For basic exceptions
                        message = ex.Message;
                    }
                }
                else
                {
                    // Exception is null. Interesting... NTD
                }
                // Construct response
                var response = new ServerResponseGeneric<Object>(false)
                {
                    Data = message,
                    Code = responseCode,
                };
                // Set return type to JSON
                context.Response.ContentType = $"{HttpConst.ContentTypeJson}; {Constants.CharsetUtf8}";
                // Set status code
                context.Response.StatusCode = statusCode;
                // SPA framework very strict on response header. Ignores data if these are missing
                context.Response.Headers.Add(HttpConst.CorsOrigin, new[] { HttpConst.AcceptedOrigin });
                context.Response.Headers.Add(HttpConst.CorsHeaders, new[] { HttpConst.AcceptedHeaders });
                context.Response.Headers.Add(HttpConst.CorsMethods, new[] { HttpConst.AcceptedMethods });
                context.Response.Headers.Add(HttpConst.CorsCredentials, new[] { Boolean.TrueString });
                // Send response
                await context.Response.WriteAsync(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase), Encoding.UTF8);
            });
        }
    }
}