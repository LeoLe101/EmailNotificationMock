using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmailServer
{
    [ApiController]
    // [Authorize]
    [Route(ApiRoutesV1.EmailV1)]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailController> _logger;

        // Constructor of the email controller to perform dependency injection
        public EmailController(IEmailRepository emailRepository, ILogger<EmailController> logger)
        {
            _logger = logger;
            _emailRepository = emailRepository;
        }

        // GET: v1/email
        /*
         * Get 2 different kind of list: EmailTriggerPair list | ScheduledEmail list
         */
        [HttpGet]
        public async Task<ServerResponseBase> GetEmailList(GetListRequest request)
        {
            return new ServerResponseEmailListView(await _emailRepository.GetList(request));
        }

        // POST: v1/email
        /*
         * Schedule a new email notification from the requested information about the email
         * from the inspector and the user.
         */
        [HttpPost]
        public async Task<ServerResponseBase> ScheduleEmail(Email request)
        {
            return new ServerResponseScheduledEmail(await _emailRepository.ScheduleEmail(request));
        }

        // PUT: v1/email/3
        /*
         * Update the specified scheduled email from the server
         */
        [HttpPut(ApiRoutesV1.EmailId)]
        public async Task<ServerResponseBase> UpdateScheduledEmail([FromHeader] string id, [FromBody] Email request)
        {
            // Check if the id given is the same with the id from the request
            if (!StringUtil.AreEqual(id, request?.Id))
            {
                return new ServerResponseMessage(false, $"Email Notification ");
            }

            // Update the Scheduled Email
            await _emailRepository.UpdateScheduledEmail(request);

            return new ServerResponseBase(true)
            {
                Code = Constants.StrOk
            };
        }

        // DELETE: v1/email/3
        /*
         * Delete the Scheduled Email (Soft Delete at the moment due to authentication is not implemented yet)
         */
        [HttpDelete(ApiRoutesV1.EmailId)]
        public async Task<ServerResponseBase> DeleteScheduleEmail([FromRoute] string id)
        {
            await _emailRepository.SoftDelete(id); // Not completely deleted in the DB
            // await _emailRepository.Delete(id); // Completely delete the data in the DB

            return new ServerResponseBase(true)
            {
                Code = Constants.StrOk
            };
        }

    }
}