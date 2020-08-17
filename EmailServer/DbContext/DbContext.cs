using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace EmailServer
{

    /// <summary>
    /// Application DBContext: Create a new database if it's not already existed.
    /// The DBContext will handle all of the CRUD operations from the server into the SQL Server DB
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        private IHttpContextAccessor _accessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor accessor
        ) : base(options)
        {
            _accessor = accessor;
        }

        public DbSet<DbMail> Emails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbMail>().Property(x => x.Id).HasColumnName("Id").IsRequired(); 
            builder.Entity<DbMail>().Property(x => x.ConcurrencyStamp).HasDefaultValueSql("NEWID()");
            builder.Entity<DbMail>().Property(x => x.CreatedTime).HasColumnName("CreatedTime").HasDefaultValueSql("GETDATE()"); 
            builder.Entity<DbMail>().HasIndex(x => x.JobId);
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            _saveChanges();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _saveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _saveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            _saveChanges();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void _saveChanges()
        {
            // Get all changes
            var entries = ChangeTracker.Entries<IDbBase>();

            if (entries != null)
            {
                DateTime Now = DateTime.UtcNow;
                string CurrentUser = _getCurrentUser(); // Fill in with logic to get username

                foreach (var item in entries)
                {
                    // Update on modified
                    if (item.State == EntityState.Added)
                    {
                        item.Entity.Active = true;
                        item.Entity.CreatedTime = Now;
                        item.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
                    }
                    else if (item.State == EntityState.Modified)
                    {
                        item.Entity.ConcurrencyStamp = Guid.NewGuid().ToString();
                    }
                }
            }
        }

        private string _getCurrentUser()
        {
            var user = _accessor?.HttpContext?.User?.Identity?.Name;
            return !String.IsNullOrWhiteSpace(user) ? user : "Anonymous";
        }
    }
}