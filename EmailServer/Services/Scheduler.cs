using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace EmailServer
{

    /// <summary>
    /// Schedule Job for the Quartz DLL Library.
    /// This class is going to implement the execution of the specified
    /// email provided through the constructor
    /// </summary>
    public static class Scheduler
    {

        public static async Task ScheduleEmailAsync(DateTime scheduleTime, Email details)
        {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ScheduleJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartAt(scheduleTime)
            .WithPriority(1)
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

    }
}