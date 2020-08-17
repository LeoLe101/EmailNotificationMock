using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmailServer
{

    // Basic Response
    public class ServerResponseBase
    {
        public bool Success { get; protected set; }

        public string Code { get; set; }

        public ServerResponseBase(bool isSuccessful)
        {
            Success = isSuccessful;
        }
    }

    // Base Generic Class
    public class ServerResponseGeneric<T> : ServerResponseBase
    {
        public T Data { get; set; }

        public ServerResponseGeneric(bool isSuccessful) : base(isSuccessful) { }
    }

    // Base Generic List Class
    public class ServerResponseBaseList<T> : ServerResponseGeneric<IList<T>>
    {
        public ServerResponseBaseList() : base(true) { }
    }


    // Response for GET: Email and Trigger time pair list requests
    public class ServerResponseEmailListView : ServerResponseBaseList<EmailListView>
    {
        public ServerResponseEmailListView(IList<EmailListView> response) : base()
        {
            Data = response;
        }
    }

    // Response for POST: schedule Email Notification
    public class ServerResponseScheduledEmail : ServerResponseGeneric<Email>
    {
        public ServerResponseScheduledEmail(Email response) : base(true)
        {
            Data = response;
        }
    }

    // Response for generic error/msg from the server to the client
    public class ServerResponseMessage : ServerResponseGeneric<string>
    {
        public ServerResponseMessage(Boolean isSuccess, string response) : base(isSuccess)
        {
            Data = response;
        }
    }

    public class ServerResponseValidationError : ServerResponseGeneric<IList<ValidationError>>
    {
        public ServerResponseValidationError(ModelStateDictionary modelState) : base(false)
        {
            Code = ErrorCodes.BadRequest;
            Data = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                .ToList();
        }
    }

    public class ValidationError
    {
        public string field { get; }

        public string message { get; }

        public ValidationError(string inField, string inMsg)
        {
            field = !String.IsNullOrWhiteSpace(inField) ? inField : null;
            message = inMsg;
        }
    }


}