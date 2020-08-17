using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Utf8Json;
using Utf8Json.Resolvers;

namespace EmailServer
{
    public static class CustomMiddlewareHandler
    {

        // public delegate void Callback(int i);

        // public static async Task Invoke(HttpContext context)
        // {
        //     try
        //     {
        //         // await Callback(500);
        //     }
        //     catch (Exception ex)
        //     {
        //         await _HandleExceptionAsync(context, ex);
        //     }
        // }

        // private static void _HandleExceptionAsync(HttpContext context, Exception ex)
        // {
            
        //     // Init message & code
        //     Object message = "Unhandled exception";
        //     string responseCode = ErrorCodes.ServerError;
        //     // Set default status code to server error
        //     int statusCode = StatusCodes.Status500InternalServerError;
        //     // Extract exception type
        //     var exType = ex.GetType();
        //     // Get custom message based on exception type
        //     if (exType == typeof(DbUpdateConcurrencyException))
        //     {
        //         // Casting the exception to its proper type for data extraction
        //         DbUpdateConcurrencyException scopedEx = (ex as DbUpdateConcurrencyException);
        //         // Client value
        //         var clValue = scopedEx?.Entries.FirstOrDefault().Entity;
        //         // Database value
        //         var dbValue = (await scopedEx?.Entries.FirstOrDefault().GetDatabaseValuesAsync()).ToObject();
        //         message = new { client = clValue, database = dbValue };
        //         responseCode = ErrorCodes.EntityChanged;
        //         statusCode = StatusCodes.Status409Conflict;
        //     }
        //     else if (exType == typeof(DbUpdateException))
        //     {
        //         // Db exceptions seem to have better details in the inner exception
        //         message = ex?.InnerException?.Message ?? ex.Message;
        //     }
        //     else if (exType == typeof(ApiForbiddenException))
        //     {
        //         // Casting the exception to its proper type for data extraction
        //         ApiForbiddenException scopedEx = (ex as ApiForbiddenException);
        //         message = scopedEx.Message ?? "Top secret";
        //         responseCode = scopedEx.Code ?? ErrorCodes.Forbidden;
        //         statusCode = StatusCodes.Status403Forbidden;
        //     }
        //     else if (exType == typeof(ApiUnauthorizedException))
        //     {
        //         // Casting the exception to its proper type for data extraction
        //         ApiUnauthorizedException scopedEx = (ex as ApiUnauthorizedException);
        //         message = scopedEx.Message ?? "Shoo!!! Not allowed";
        //         responseCode = scopedEx.Code ?? ErrorCodes.Unauthorized;
        //         statusCode = StatusCodes.Status401Unauthorized;
        //     }
        //     else if (exType == typeof(ApiInvalidModelException))
        //     {
        //         // Casting the exception to its proper type for data extraction
        //         ApiInvalidModelException scopedEx = (ex as ApiInvalidModelException);
        //         message = scopedEx.Message ?? "Semi naked entity";
        //         responseCode = scopedEx.Code ?? ErrorCodes.BadRequest;
        //         statusCode = StatusCodes.Status400BadRequest;
        //     }
        //     else if (exType == typeof(ApiUnprocessableException))
        //     {
        //         // TODO: Potentially extract errors out of this exception
        //         // Casting the exception to its proper type for data extraction
        //         ApiUnprocessableException scopedEx = (ex as ApiUnprocessableException);
        //         message = scopedEx.Message ?? "Error completing request";
        //         responseCode = scopedEx.Code ?? ErrorCodes.UnprocessableRequest;
        //         statusCode = StatusCodes.Status422UnprocessableEntity;
        //     }
        //     else if (exType == typeof(ApiNotFoundException))
        //     {
        //         // Casting the exception to its proper type for data extraction
        //         ApiNotFoundException scopedEx = (ex as ApiNotFoundException);
        //         message = scopedEx.Message ?? "It's gone or never here";
        //         responseCode = scopedEx.Code ?? ErrorCodes.NotFound;
        //         statusCode = StatusCodes.Status404NotFound;
        //     }
        //     else if (exType == typeof(KeyNotFoundException))
        //     {
        //         // Custom exception testing
        //         message = ex.Message ?? "I'm blind";
        //         responseCode = ErrorCodes.NotFound;
        //         statusCode = StatusCodes.Status404NotFound;
        //     }
        //     else if (exType == typeof(NotImplementedException))
        //     {
        //         // Custom exception testing
        //         message = "Under construction";
        //     }
        //     else
        //     {
        //         // For basic exceptions
        //         message = ex.Message;
        //     }

        //     // Construct response
        //     var response = new ServerResponseGeneric<Object>(false)
        //     {
        //         Data = message,
        //         Code = responseCode,
        //     };
        //     // Set return type to JSON
        //     context.Response.ContentType = $"{HttpConst.ContentTypeJson}; {Constants.CharsetUtf8}";
        //     // Set status code
        //     context.Response.StatusCode = statusCode;
        //     // SPA framework very strict on response header. Ignores data if these are missing
        //     context.Response.Headers.Add(HttpConst.CorsOrigin, new[] { HttpConst.AcceptedOrigin });
        //     context.Response.Headers.Add(HttpConst.CorsHeaders, new[] { HttpConst.AcceptedHeaders });
        //     context.Response.Headers.Add(HttpConst.CorsMethods, new[] { HttpConst.AcceptedMethods });
        //     context.Response.Headers.Add(HttpConst.CorsCredentials, new[] { Boolean.TrueString });
        //     // Send response
        //     await context.Response.WriteAsync(JsonSerializer.ToJsonString(response, StandardResolver.CamelCase), Encoding.UTF8);
        //     return;
        // }

    }
}