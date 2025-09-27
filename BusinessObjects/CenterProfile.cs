using Core.Base;

namespace BusinessObjects
{
    public class CenterProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public string LicenseIssuedBy { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        public virtual ICollection<TeacherProfile> TeacherProfiles { get; set; } = new List<TeacherProfile>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}


