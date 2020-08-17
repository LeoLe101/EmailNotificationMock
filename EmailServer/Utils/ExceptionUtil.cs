using System;
using System.Runtime.Serialization;

namespace EmailServer
{
    /// <summary>
    /// Base class for Api exception
    /// </summary>
    public class ApiException : Exception
    {
        public string Code { get; }

        public ApiException(string responseCode)
        {
            Code = responseCode;
        }

        public ApiException(string message, string responseCode) : base(message)
        {
            Code = responseCode;
        }

        public ApiException(string message, Exception innerException, string responseCode) : base(message, innerException)
        {
            Code = responseCode;
        }

        public ApiException(SerializationInfo info, StreamingContext context, string responseCode) : base(info, context)
        {
            Code = responseCode;
        }
    }

    /// <summary>
    /// Forbidden exception for authorization issues (AuthZ: blocking role/policy based restrictions in internal apis)
    /// </summary>
    public class ApiForbiddenException : ApiException
    {
        public ApiForbiddenException() : base(ErrorCodes.Forbidden) { }

        public ApiForbiddenException(string message, string responseCode = ErrorCodes.Forbidden) : base(message, responseCode) { }

        public ApiForbiddenException(string message, Exception innerException, string responseCode = ErrorCodes.Forbidden) : base(message, innerException, responseCode) { }

        public ApiForbiddenException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.Forbidden) : base(info, context, responseCode) { }
    }

    /// <summary>
    /// Unauthorized exception for authentication issues (AuthN: blocking basic auth based restrictions in internal apis)
    /// </summary>
    public class ApiUnauthorizedException : ApiException
    {
        public ApiUnauthorizedException() : base(ErrorCodes.Unauthorized) { }

        public ApiUnauthorizedException(string message, string responseCode = ErrorCodes.Unauthorized) : base(message, responseCode) { }

        public ApiUnauthorizedException(string message, Exception innerException, string responseCode = ErrorCodes.Unauthorized) : base(message, innerException, responseCode) { }

        public ApiUnauthorizedException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.Unauthorized) : base(info, context, responseCode) { }
    }

    /// <summary>
    /// Missing or invalid request properties
    /// </summary>
    public class ApiInvalidModelException : ApiException
    {
        public ApiInvalidModelException() : base(ErrorCodes.BadRequest) { }

        public ApiInvalidModelException(string message, string responseCode = ErrorCodes.BadRequest) : base(message, responseCode) { }

        public ApiInvalidModelException(string message, Exception innerException, string responseCode = ErrorCodes.BadRequest) : base(message, innerException, responseCode) { }

        public ApiInvalidModelException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.BadRequest) : base(info, context, responseCode) { }
    }

    /// <summary>
    /// Concurrency exception for failed db updates due to stale data
    /// </summary>
    public class ApiDbConcurrencyException : ApiException
    {
        public ApiDbConcurrencyException() : base(ErrorCodes.EntityChanged) { }

        public ApiDbConcurrencyException(string message, string responseCode = ErrorCodes.EntityChanged) : base(message, responseCode) { }

        public ApiDbConcurrencyException(string message, Exception innerException, string responseCode = ErrorCodes.EntityChanged) : base(message, innerException, responseCode) { }

        public ApiDbConcurrencyException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.EntityChanged) : base(info, context, responseCode) { }
    }

    /// <summary>
    /// Unprocessable exception for random process issues
    /// </summary>
    public class ApiUnprocessableException : ApiException
    {
        public Object error { get; }

        public ApiUnprocessableException() : base(ErrorCodes.UnprocessableRequest) { }

        public ApiUnprocessableException(string message, string responseCode = ErrorCodes.UnprocessableRequest) : base(message, responseCode) { }

        public ApiUnprocessableException(string message, Object exError, string responseCode = ErrorCodes.UnprocessableRequest) : base(message, responseCode)
        {
            error = exError;
        }

        public ApiUnprocessableException(string message, Exception innerException, string responseCode = ErrorCodes.UnprocessableRequest) : base(message, innerException, responseCode) { }

        public ApiUnprocessableException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.UnprocessableRequest) : base(info, context, responseCode) { }
    }

    /// <summary>
    /// Not found exception for null return on queries
    /// </summary>
    public class ApiNotFoundException : ApiException
    {
        public ApiNotFoundException() : base(ErrorCodes.NotFound) { }

        public ApiNotFoundException(string message, string responseCode = ErrorCodes.NotFound) : base(message, responseCode) { }

        public ApiNotFoundException(string message, Exception innerException, string responseCode = ErrorCodes.NotFound) : base(message, innerException, responseCode) { }

        public ApiNotFoundException(SerializationInfo info, StreamingContext context, string responseCode = ErrorCodes.NotFound) : base(info, context, responseCode) { }
    }


}
