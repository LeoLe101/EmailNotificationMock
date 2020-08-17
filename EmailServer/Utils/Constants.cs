namespace EmailServer
{
    public static class Constants
    {
        // Common constants
        public const string ProductName = "EmailNotification";
        public const string StrOk = "OK";
        public const string StrY = "Y";
        public const string StrN = "N";
        public const string StrUnknown = "Unknown";
        public const string StrOrigin = "Origin";
        public const string ContentTypeHtml = "text/html";
        public const string CharsetUtf8 = "charset=utf-8";

        // Auth constants
        public const string JwtConfirmed = "confirmed";
        public const string StrBearer = "Bearer";
        public const string StrAuthorization = "Authorization";
        public const string WeekSpanTokenProvider = "WeekSpanTokenProvider";
        public const string VerifyAccountToken = "VerifyAccountToken";
        public const string ActivateAccountToken = "ActivateAccountToken";

        // Exception constants
        public const string ErrorForbidden = "Account denied";
        public const string ErrorUnauthorized = "Access denied";
        public const string ErrorMissingProp = "Required prop missing";
        public const string ErrorInvalidProp = "Required prop invalid";
        public const string ErrorNoMatch = "Route does not match request";
        public const string ErrorUnprocessable = "Unable to process request";
        public const string ErrorNotFound = "Entity not found";
    }

    public static class HttpConst
    {
        // Header constants
        public const string CorsOrigin = "Access-Control-Allow-Origin";
        public const string CorsHeaders = "Access-Control-Allow-Headers";
        public const string CorsMethods = "Access-Control-Allow-Methods";
        public const string CorsCredentials = "Access-Control-Allow-Credentials";
        public const string ContentTypeXml = "application/xml";
        public const string ContentTypeJson = "application/json";
        public const string MethodOptions = "OPTIONS";
        public const string AcceptedOrigin = "Origin";
        public const string AcceptedHeaders = "Access-Control-Allow-Origin, authorization, Content-Type, Accept";
        public const string AcceptedMethods = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
    }

    public static class Defaults
    {
        public const int PaginationLimit = 50;
        public const int AccessCodeLength = 8;
    }

    public static class ErrorCodes
    {
        public const string BadRequest = "bad_request";
        public const string Unauthorized = "unauthorized";
        public const string Forbidden = "forbidden";
        public const string NotFound = "not_found";
        public const string UnprocessableRequest = "unprocessable_request";
        public const string ServerError = "server_error";
        public const string EntityChanged = "entity_changed";
    }

    public static class Policies
    {
        public const string AllAdminsOnly = "AdminsOnly";
        public const string GlobalAdminsOnly = "GlobalAdminsOnly";
        public const string IsConfirmed = "IsConfirmed";
    }

    /// <summary>
    /// Email System basic routes
    /// </summary>
    public static class ApiRoutesV1
    {
        public const string LetsEncrypt = ".well-known/acme-challenge/{id}";
        public const string EmailV1 = "v1/email";
        public const string EmailId = "{id?}";
    }

    
}