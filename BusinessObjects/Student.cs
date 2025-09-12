using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Student : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public string? ParentId { get; set; }
        public virtual Parent? Parent { get; set; }

        // Business Properties
        [Required]
        [MaxLength(100)]
        public string StudentId { get; set; } = string.Empty; //ma  hoc sinh

        [MaxLength(200)]
        public string? SchoolName { get; set; }

        [MaxLength(100)]
        public string? Class { get; set; }

        public StudentGroup StudentGroup { get; set; }

        public bool IsEligibleForInSchool { get; set; } = false;

        public bool IsEligibleForOutOfSchool { get; set; } = true;

        [MaxLength(500)]
        public string? AcademicNotes { get; set; }

        // Collection Navigation Properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
