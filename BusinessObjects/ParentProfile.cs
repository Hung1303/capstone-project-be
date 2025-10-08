using Core.Base;

namespace BusinessObjects
{
    public class ParentProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Address { get; set; }
        public string? PhoneSecondary { get; set; }

        public virtual ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();
    }

}


