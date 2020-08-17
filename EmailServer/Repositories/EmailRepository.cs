using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EmailServer
{
    /// <summary>
    /// Email Notification services
    /// </summary>
    public class EmailRepository : IEmailRepository
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<IEmailRepository> _logger;
        private readonly SendGridEmailServiceOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailRepository(
            ApplicationDbContext dbContext,
            ILogger<IEmailRepository> logger,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SendGridEmailServiceOptions> optionsAccessor
        )
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._options = optionsAccessor.Value;
            this._httpContextAccessor = httpContextAccessor;
        }

        #region CRUD Functions

        public async Task<Email> ScheduleEmail(Email request)
        {
            _logger.LogInformation("---------- Email_ScheduleEmail ----------");

            // Send the email to both sender and receiver about the notification
            await SendEmailAsync(request);

            // Create a new DbMail entity for incoming client request
            DbMail entity = new DbMail();
            entity.AdaptToDb(request);

            // Add the newly created entity into the database
            await _dbContext.Emails.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            // Normalize it before returning
            return Email.AdaptFromDb(entity);
        }

        public async Task<List<EmailListView>> GetList(GetListRequest request)
        {
            _logger.LogInformation("---------- Email_GetList ----------");

            // Check if the company ID existed to extract the list of email scheduled for that company
            if (String.IsNullOrWhiteSpace(request?.CompanyID))
            {
                throw new ApiInvalidModelException($"Email_GetList: {Constants.ErrorMissingProp}", "no_company_id");
            }
            _logger.LogInformation("---- Company ID: " + request.CompanyID);

            Guid? companyId = IdUtil.GetId(request.CompanyID);


            // Query all email that still active, scheduled, and belong to that company 
            IQueryable<DbMail> query = _dbContext.Emails
                .Where(i => i.Active == true)
                .Where(i => i.Id == companyId);

            // Normalize each props and execute the query into desire list
            List<EmailListView> resultList = await query.Select(e => new EmailListView
            {
                Id = e.Id.ToString(),
                ToEmail = e.ToEmail,
                FromEmail = e.FromEmail,
                Subject = e.Subject,
                ScheduledJob = new TimeIdPair
                {
                    JobId = e.JobId,
                    ScheduledTime = e.ScheduledTime
                },
                Status = e.Status,
            }).ToListAsync();

            _logger.LogInformation("---- Get List: " + resultList);

            return null;
        }

        public async Task<Email> GetScheduledEmail(string Id)
        {
            _logger.LogInformation("---------- Email_GetScheduledEmailAsync ----------");

            DbMail scheduledEmail = await _GetDbEntity(Id);

            return Email.AdaptFromDb(scheduledEmail);
        }

        public async Task<Email> UpdateScheduledEmail(Email request)
        {
            _logger.LogInformation("---------- Email_UpdateScheduledEmail ----------");

            // Ensure id exists (ValidateNormalizeRequest is also used w/ create, so it doesnt enforce id required)
            if (String.IsNullOrWhiteSpace(request?.Id))
            {
                throw new ApiInvalidModelException($"Email_UpdateScheduledEmail: {Constants.ErrorMissingProp}", "no_email_id");
            }

            // Validate request and handle relational property complexities
            request = await _ValidateNormalizeEntityRequest(request);

            // Guaranteed to return dbEntity. Throws exceptions otherwise
            DbMail entity = await _GetDbEntity(request.Id);

            // Begin updates
            entity.AdaptToDb(request);

            // Start tracking entity
            _dbContext.Emails.Update(entity);

            // Commit to db
            await _dbContext.SaveChangesAsync();

            return Email.AdaptFromDb(entity);
        }

        public async Task<Email> UpdateEmailStatusAsync(Email details, EmailStatus stat)
        {

            _logger.LogInformation("---------- Email_UpdateEmailStatusAsync ----------");

            // No ID of the scheduled email found, throw error
            if (String.IsNullOrWhiteSpace(details?.Id))
            {
                throw new ApiInvalidModelException($"Email_UpdateEmailStatusAsync: {Constants.ErrorMissingProp}", "no_email_id");
            }

            // Guaranteed to return dbEntity. Throws exceptions otherwise
            DbMail entity = await _GetDbEntity(details.Id);
            entity.Status = stat;
            entity.AdaptToDb(details);

            // Start tracking entity
            _dbContext.Emails.Update(entity);

            // Commit to db
            await _dbContext.SaveChangesAsync();

            // Return the normalized Email
            return Email.AdaptFromDb(entity);
        }

        #endregion

        #region Helper

        // Send the email to the client
        public Task SendEmailAsync(Email request)
        {
            _logger.LogInformation($"---------- Email_Helper_SendEmailAsync ---------- ");

            SendGridClient server = new SendGridClient(_options.ApiKey);
            SendGridMessage email = new SendGridMessage()
            {
                From = new EmailAddress(_options.FromEmail, _options.FromName),
                Subject = request.Subject,
                PlainTextContent = request.Subject,
                HtmlContent = request.Content,
            };
            email.AddTo(new EmailAddress(request.ToEmail, request.ToName));

            // Send the mail
            try
            {
                // Try sending email async
                return server.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                // Need to catch and continue as this error should not crash the server
                // Just log error
                _logger.LogError($"---------- Email_Execute (Send Email Async): {ex.Message} ---------- ");

                // Send error back up so we know which method called this
                // NOTE: Most email calling methods will catch this error and just log them without failing the request
                throw;
            }
        }

        // Make the Scheduled Email in-active instead of completely deleted it
        public async Task SoftDelete(string id)
        {
            _logger.LogInformation("---------- Email_Helper_SoftDelete ----------");

            // Guaranteed to return dbEntity. Throws exceptions otherwise
            DbMail entity = await _GetDbEntity(id);

            // Entity found, soft delete it (set active to false)
            entity.Active = false;
            entity.Status = EmailStatus.InActive;

            // Start tracking entity
            _dbContext.Emails.Update(entity);

            // Commit to db
            await _dbContext.SaveChangesAsync();
        }

        // Delete the scheduled email permanently
        public async Task Delete(string id)
        {
            _logger.LogInformation("---------- Email_Helper_Delete ----------");

            // Guaranteed to return dbEntity. Throws exceptions otherwise
            DbMail entity = await _GetDbEntity(id);

            // Start tracking entity
            _dbContext.Emails.Remove(entity);

            // Commit to db
            await _dbContext.SaveChangesAsync();
        }

        // Global admins only
        private async Task<DbMail> _GetDbEntity(string id)
        {
            _logger.LogInformation("---------- Email_Helper_GetDbEntity ----------");

            // Ensure id exists
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ApiInvalidModelException($"Email_GetDbEntity: {Constants.ErrorMissingProp}", "no_email_id");
            }

            // Standardize request object
            Guid? emailId = IdUtil.GetId(id);

            // Init result
            DbMail entity = null;

            // Construct base query
            IQueryable<DbMail> query = _dbContext.Emails.AsNoTracking();

            // Query for email & add filters
            entity = await query
                .Where(i => i.Active == true)
                .Where(i => i.Id == emailId)
                .FirstOrDefaultAsync();

            // Ensure entity exists
            if (entity == null)
            {
                throw new ApiNotFoundException($"Email_GetDbEntity: {Constants.ErrorNotFound}", "no_email");
            }

            // Return DbEntity
            return entity;
        }

        // Get the template of the email if there's any
        private async Task<string> _LoadEmailTemplate(string templateName)
        {
            _logger.LogInformation("---------- Email_Helper_LoadEmailTemplate ----------");

            // Init return val
            string templateText = default;

            using (var reader = FileUtil.GetEmailTemplate(templateName))
            {
                // Read file contents
                templateText = await reader.ReadToEndAsync();
            }

            if (String.IsNullOrWhiteSpace(templateText))
            {
                throw new ApiUnprocessableException($"LoadEmailTemplate: {Constants.ErrorUnprocessable}", "load_email_template_failed");
            }

            return templateText;
        }

        // Validate and normalize all properties from incoming request
        private async Task<Email> _ValidateNormalizeEntityRequest(Email request)
        {
            _logger.LogInformation("---------- Email_Helper_ValidateNormalizeEntityRequest ----------");

            if (String.IsNullOrWhiteSpace(request.ToName))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email To Name]");
            }

            if (String.IsNullOrWhiteSpace(request.ToEmail))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email To Name]");
            }

            if (String.IsNullOrWhiteSpace(request.FromName))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email To Name]");
            }

            if (String.IsNullOrWhiteSpace(request.FromName))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email To Name]");
            }

            if (String.IsNullOrWhiteSpace(request.Subject))
            {
                request.Subject = "Cylindex - Inspection Reminder";
            }

            if (String.IsNullOrWhiteSpace(request.Content))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email Content]");
            }

            if (String.IsNullOrWhiteSpace(request.CompanyId))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email Company ID]");
            }

            if (String.IsNullOrWhiteSpace(request.CompanyInfo))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email Company Information]");
            }

            if (String.IsNullOrWhiteSpace(request.JobId.ToString()))
            {
                throw new ApiInvalidModelException($"Email_ValidateNormalizeEntityRequest: {Constants.ErrorMissingProp} [Email TriggerID]");
            }

            request.IsHTML = request.IsHTML ? request.IsHTML : false;

            return request;
        }

        #endregion

        #region Testing
        public async Task<List<DbMail>> GetListTest(GetListRequest request)
        {
            _logger.LogInformation("---------- Email_GetListTest ----------");

            return await _dbContext.Emails.ToListAsync();
        }
        #endregion




    }

    /// <summary>
    /// Interface CRUD for Email Repository
    /// </summary>    
    public interface IEmailRepository
    {
        // CRUD interfaces
        Task<Email> ScheduleEmail(Email request);

        Task<List<EmailListView>> GetList(GetListRequest request);

        Task<Email> GetScheduledEmail(string id);

        Task<Email> UpdateScheduledEmail(Email request);

        Task<Email> UpdateEmailStatusAsync(Email details, EmailStatus status);

        // TESTING
        Task<List<DbMail>> GetListTest(GetListRequest request);

        // Helper interfaces
        Task SendEmailAsync(Email request);

        Task SoftDelete(string id);

        Task Delete(string id);
    }
}