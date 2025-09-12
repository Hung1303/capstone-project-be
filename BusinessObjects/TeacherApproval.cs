using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class TeacherApproval : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;

        [Required]
        public string AdminId { get; set; }
        public virtual Admin Admin { get; set; } = null!;

        // Business Properties
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        public DateTime? ReviewedAt { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        public bool RequiresDocumentation { get; set; } = false;

        [MaxLength(1000)]
        public string? RequiredDocuments { get; set; }
    }
}
