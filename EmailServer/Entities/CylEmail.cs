using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Quartz;

namespace EmailServer
{
    public class BaseEntity
    {
        [Required]
        public string Id { get; set; }

        [ConcurrencyCheck, MaxLength(256)]
        public string ConcurrencyStamp { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }
    }

    public class BaseEmailServiceOptions : BaseEntity
    {
        [Required]
        public string ToName { get; set; }

        [
            Required,
            MaxLength(256),
            DataType(DataType.EmailAddress)
        ]
        public string ToEmail { get; set; }

        [Required]
        public string FromName { get; set; }

        [
            Required,
            MaxLength(256),
            DataType(DataType.EmailAddress)
        ]
        public string FromEmail { get; set; }

        [MaxLength(256)]
        public string DefaultBcc { get; set; }

        public string Subject { get; set; }

        [Required, EnumDataType(typeof(EmailStatus))]
        public EmailStatus Status { get; set; }
    }


    public class EmailListView
    {
        public string Id { get; set; }

        public string ToEmail { get; set; }

        public string FromEmail { get; set; }

        public string Subject { get; set; }

        public TimeIdPair ScheduledJob { get; set; }

        public EmailStatus Status { get; set; }
    }

    public class TimeIdPair
    {
        public string JobId { get; set; }

        public DateTime ScheduledTime { get; set; }
    }


    public class SendGridEmailServiceOptions : BaseEmailServiceOptions
    {
        public string ApiKey { get; set; }
    }

    public class Email : BaseEmailServiceOptions
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public bool IsHTML { get; set; }

        [Required]
        public string CompanyInfo { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [Required]
        public string JobId { get; set; }

        public static Email AdaptFromDb(DbMail dbEntity, bool includeAll = true)
        {
            // Ensure dbEntity is valid
            if (dbEntity == null)
            {
                return null;
            }
            else
            {
                Email entity = new Email
                {
                    Id = dbEntity.Id.ToString(),
                    ToName = dbEntity.ToName,
                    ToEmail = dbEntity.ToEmail,
                    FromName = dbEntity.FromName,
                    FromEmail = dbEntity.FromEmail,
                    DefaultBcc = dbEntity.DefaultBcc,
                    Subject = dbEntity.Subject,
                    Status = dbEntity.Status,
                    Content = dbEntity.Content,
                    IsHTML = dbEntity.IsHTML,
                    CompanyInfo = dbEntity.CompanyInfo,
                    CompanyId = dbEntity.CompanyId,
                    JobId = dbEntity.JobId
                };
                // Partial adaptation includes just the basic. Used in relational adaptations
                if (includeAll)
                {
                    entity.ConcurrencyStamp = dbEntity.ConcurrencyStamp;
                    entity.Active = dbEntity.Active;
                    entity.ScheduledTime = dbEntity.ScheduledTime;
                    entity.CreatedTime = dbEntity.CreatedTime;
                }

                return entity;
            }
        }
    }
}
