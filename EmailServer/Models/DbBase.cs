using System;
using System.ComponentModel.DataAnnotations;

namespace EmailServer
{
    /// <summary>
    /// Base model for other DB model class
    /// </summary>
    public abstract class DbBase : IDbBase
    {
        [Key]
        public Guid Id { get; set; }

        [ConcurrencyCheck, MaxLength(256)]
        public string ConcurrencyStamp { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }

        [Required]
        public DateTime CreatedTime { get; set; }
    }

    public interface IDbBase
    {
        Guid Id { get; set; }

        string ConcurrencyStamp { get; set; }

        bool Active { get; set; }

        DateTime ScheduledTime { get; set; }

        DateTime CreatedTime { get; set; }
    }
}