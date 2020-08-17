using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmailServer
{
    public class MockData
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                // Get DB context and create it
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureCreated();
                //context.Database.Migrate();

                if (context.Emails != null && context.Emails.Any())
                    return;

                var emailList = MockData.GetDBMails(context).ToArray();
                context.Emails.AddRange(emailList);
                context.SaveChanges();
            };
        }

        // Data coming from the client to schedule the email
        public static List<DbMail> GetDBMails(ApplicationDbContext dbContext)
        {
            Random rand = new Random();
            List<DbMail> emailList = new List<DbMail>
            {
                new DbMail
                {
                    Id = new Guid(),
                    Active = true,
                    ScheduledTime = DateTime.Now.AddMinutes(rand.Next(0,10)),
                    CreatedTime = DateTime.Now,
                    ConcurrencyStamp = "A",
                    ToName = "Phuc Le",
                    FromName = "Leo Le",
                    ToEmail = "phucsupreme@gmail.com",
                    FromEmail = "leolemain@gmail.com",
                    DefaultBcc = "phucle@uw.edu",
                    Subject = "ContextDB Test 1 for database migration",
                    Content = "This is a test message!",
                    Status = EmailStatus.Pending,
                    IsHTML = true,
                    CompanyInfo = "A Company",
                    CompanyId = "b93ab937-1df8-414d-8b0e-4f55529bac7d",
                    JobId = "1"
                },

                new DbMail
                {
                    Id = new Guid(),
                    Active = true,
                    ScheduledTime = DateTime.Now.AddMinutes(rand.Next(0,10)),
                    CreatedTime = DateTime.Now.AddYears(-1),
                    ConcurrencyStamp = "A",
                    ToName = "Don Kinney",
                    FromName = "Leo Le",
                    ToEmail = "phucsupreme@gmail.com",
                    FromEmail = "leolemain@gmail.com",
                    DefaultBcc = "phucle@uw.edu",
                    Subject = "ContextDB Test 2 for database stability",
                    Content = "I don't know what to say",
                    Status = EmailStatus.Pending,
                    IsHTML = true,
                    CompanyInfo = "B Company",
                    CompanyId = "924ddeb5-26c5-497a-91e7-bc456cedbd52",
                    JobId = "2"
                },

                new DbMail
                {
                    Id = new Guid(),
                    Active = true,
                    ScheduledTime = DateTime.Now.AddMinutes(rand.Next(0,10)),
                    CreatedTime = DateTime.Now.AddYears(-3).AddMonths(5),
                    ConcurrencyStamp = "A",
                    ToName = "Banana 4 Ur Mama",
                    FromName = "Leo Le",
                    ToEmail = "phucsupreme@gmail.com",
                    FromEmail = "leolemain@gmail.com",
                    DefaultBcc = "phucle@uw.edu",
                    Subject = "ContextDB Test 3 for database Check",
                    Content = "This is probably the only thing that I can try at the moment",
                    Status = EmailStatus.Pending,
                    IsHTML = true,
                    CompanyInfo = "C Company",
                    CompanyId = "14e0d242-2eda-4de1-8d2c-5834a9da4955",
                    JobId = "3"
                },

                new DbMail
                {
                    Id = new Guid(),
                    Active = true,
                    ScheduledTime = DateTime.Now.AddMinutes(rand.Next(0,10)),
                    CreatedTime = DateTime.Now.AddYears(-10).AddMonths(8),
                    ConcurrencyStamp = "A",
                    ToName = "Mofo PoPo",
                    FromName = "Leo Le",
                    ToEmail = "phucsupreme@gmail.com",
                    FromEmail = "leolemain@gmail.com",
                    DefaultBcc = "phucle@uw.edu",
                    Subject = "ContextDB Test 4 for checking out the Popo",
                    Content = "Watch out for the PoPo bro!",
                    Status = EmailStatus.Pending,
                    IsHTML = true,
                    CompanyInfo = "D Company",
                    CompanyId = "83783595-46c8-486f-8694-a877daa07506",
                    JobId = "4"
                },
            };
            return emailList;
        }
    }
}
