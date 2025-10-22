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

        // Location fields for mapping
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }

        public virtual ICollection<TeacherProfile> TeacherProfiles { get; set; } = new List<TeacherProfile>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}


