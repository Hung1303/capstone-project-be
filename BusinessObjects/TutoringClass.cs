using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class TutoringClass : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        [Required]
        public string TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;

        public string? ApprovedBy { get; set; }
        public virtual Admin? Approver { get; set; }

        // Business Properties
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public TutoringType Type { get; set; }

        public ClassStatus Status { get; set; } = ClassStatus.Planning;

        public TeachingMethod TeachingMethod { get; set; }

        [MaxLength(500)]
        public string? Location { get; set; }

        public int MaxStudents { get; set; } = 30;

        public int MinStudents { get; set; } = 5;

        //public decimal? TuitionFee { get; set; }

        [MaxLength(1000)]
        public string? Requirements { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? RegistrationStartDate { get; set; }

        public DateTime? RegistrationEndDate { get; set; }

        public bool IsPublicInformationPublished { get; set; } = false;

        public DateTime? PublishedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        //[MaxLength(500)]
        //public string? RejectionReason { get; set; }

        // Collection Navigation Properties
        //public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<ClassApproval> ClassApprovals { get; set; } = new List<ClassApproval>();
    }
}
