using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Quartz;

namespace EmailServer
{
    public class DbMail : DbBase
    {
        [Required, MaxLength(256)]
        public string ToName { get; set; }

        [
            Required,
            MaxLength(256),
            DataType(DataType.EmailAddress)
        ]
        public string ToEmail { get; set; }

        [Required, MaxLength(256)]
        public string FromName { get; set; }

        [
            Required,
            MaxLength(256),
            DataType(DataType.EmailAddress)
        ]
        public string FromEmail { get; set; }

        [MaxLength(256)]
        public string DefaultBcc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }

        [Required, EnumDataType(typeof(EmailStatus))]
        public EmailStatus Status { get; set; }

        public bool IsHTML { get; set; }

        public string CompanyInfo { get; set; }

        [Required]
        public string CompanyId { get; set; }

        [Required]
        public string JobId { get; set; }

        public void AdaptToDb(Email entity)
        {
            this.ToName = entity.ToName;
            this.ToEmail = entity.ToEmail;
            this.FromName = entity.FromName;
            this.FromEmail = entity.FromEmail;
            this.DefaultBcc = entity.DefaultBcc;
            this.Subject = entity.Subject;
            this.Content = entity.Content;
            this.Status = entity.Status;
            this.IsHTML = entity.IsHTML;
            this.CompanyInfo = entity.CompanyInfo;
            this.CompanyId = entity.CompanyId;
            this.JobId = entity.JobId;
        }

        public void AdaptActivationStateToDb(bool state, string concurrencyStamp)
        {
            Active = state;
            ConcurrencyStamp = concurrencyStamp;
        }
    }

}