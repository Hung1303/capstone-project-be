using Core.Base;

namespace BusinessObjects
{
    public class StudentProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public Guid? ParentProfileId { get; set; }
        public virtual ParentProfile? ParentProfile { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}


