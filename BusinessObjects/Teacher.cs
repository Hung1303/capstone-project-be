using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Teacher : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public string? ApprovedBy { get; set; }
        public virtual Admin? Approver { get; set; }

        // Business Properties
        [Required]
        [MaxLength(100)]
        public string TeacherId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? SchoolName { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [MaxLength(200)]
        public string? Qualification { get; set; }

        [MaxLength(500)]
        public string? Specialization { get; set; }

        public int YearsOfExperience { get; set; } = 0;

        public RegistrationStatus RegistrationStatus { get; set; } = RegistrationStatus.Pending;

        public DateTime? ApprovedAt { get; set; }

        public bool IsAvailableForInSchool { get; set; } = true;

        public bool IsAvailableForOutOfSchool { get; set; } = false;

        [MaxLength(1000)]
        public string? Bio { get; set; }

        // Collection Navigation Properties
        public virtual ICollection<TutoringClass> TutoringClasses { get; set; } = new List<TutoringClass>();
        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public virtual ICollection<TeacherApproval> TeacherApprovals { get; set; } = new List<TeacherApproval>();
    }
}
