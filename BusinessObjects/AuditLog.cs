using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class AuditLog : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        // Business Properties
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [Required]
        public string EntityId { get; set; }

        [Required]
        public AuditAction Action { get; set; }

        [MaxLength(2000)]
        public string? OldValues { get; set; }

        [MaxLength(2000)]
        public string? NewValues { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        //[MaxLength(45)]
        //public string? IpAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
