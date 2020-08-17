using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace EmailServer
{

    /// <summary>
    /// Schedule Job for the Quartz DLL Library.
    /// This class is going to implement the execution of the specified
    /// email provided through the constructor
    /// </summary>
    public class ScheduleJob : IJob
    {

        public Email emailNotification;
        private readonly ILogger<IJob> _logger;

        public ScheduleJob(
            Email email,
            ILogger<IJob> logger
        )
        {
            _logger = logger;
            this.emailNotification = email;
        }

        // Execute the job specified
        public Task Execute(IJobExecutionContext context)
        {
            // Check if the email notification is available or not
            if (emailNotification != null)
            { 
                // SendGridEmailService
            }
            return null;
        }
    }
}