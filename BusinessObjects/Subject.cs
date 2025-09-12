using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Subject : BaseEntity
    {
        // Business Properties
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Code { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? GradeLevel { get; set; }

        public bool IsCoreCurriculum { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Collection Navigation Properties
        public virtual ICollection<TutoringClass> TutoringClasses { get; set; } = new List<TutoringClass>();
        public virtual ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
        public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
