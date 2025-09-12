using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class ClassApproval : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string TutoringClassId { get; set; }
        public virtual TutoringClass TutoringClass { get; set; } = null!;

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

        public bool ComplianceChecked { get; set; } = false;

        public bool PublicInformationVerified { get; set; } = false;

        [MaxLength(1000)]
        public string? ComplianceNotes { get; set; }
    }
}
