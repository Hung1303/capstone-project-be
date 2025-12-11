using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class StudentProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolYear { get; set; } = string.Empty; //Example: "2024-2025"        
        [Range(6, 12, ErrorMessage = "Lớp học phải từ lớp 6 đến lớp 12.")]
        public int GradeLevel { get; set; }
        public string ClassName { get; set; }
        public Guid? ParentProfileId { get; set; }
        public virtual ParentProfile? ParentProfile { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}


