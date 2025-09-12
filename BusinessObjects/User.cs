using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class User : BaseEntity
    {
        // Business Properties
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        //[MaxLength(20)]
        //public string? NationalId { get; set; }

        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfileImageUrl { get; set; }

        // Collection Navigation Properties
        //public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
