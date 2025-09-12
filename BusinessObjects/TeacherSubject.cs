using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class TeacherSubject : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;

        [Required]
        public string SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        // Business Properties
        public bool IsPrimary { get; set; } = false;

        public int YearsOfExperience { get; set; } = 0;

        [MaxLength(500)]
        public string? Qualification { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
