using Core.Base;

namespace BusinessObjects
{
    public class CenterProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
    }
}


