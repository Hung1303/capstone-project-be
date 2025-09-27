using Core.Base;

namespace BusinessObjects
{
    public class ParentProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Address { get; set; }
        public string? PhoneSecondary { get; set; }

        public ICollection<StudentProfile> StudentProfiles = new List<StudentProfile>();
    }
}


