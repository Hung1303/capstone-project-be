using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Enrollment : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        [Required]
        public string TutoringClassId { get; set; }
        public virtual TutoringClass TutoringClass { get; set; } = null!;

        public string? ApproverId { get; set; }
        public virtual Admin? Approver { get; set; }

        // Business Properties
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;

        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        //public bool IsEligibleForInSchool { get; set; } = false;

        public StudentGroup? StudentGroup { get; set; }

        public DateTime? WithdrawalDate { get; set; }

        [MaxLength(500)]
        public string? WithdrawalReason { get; set; }

        // Collection Navigation Properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        //public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
