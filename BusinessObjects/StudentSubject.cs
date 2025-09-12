using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class StudentSubject : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        [Required]
        public string SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        // Business Properties
        public bool IsFailing { get; set; } = false;

        public decimal? LatestGrade { get; set; }

        [MaxLength(100)]
        public string? Semester { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
