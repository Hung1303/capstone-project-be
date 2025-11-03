using Core.Base;

namespace BusinessObjects
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        public UserRole Role { get; set; }
        public AccountStatus Status { get; set; } = AccountStatus.Pending;

        public TeacherProfile? TeacherProfile { get; set; }
        public CenterProfile? CenterProfile { get; set; }
        public StudentProfile? StudentProfile { get; set; }
        public ParentProfile? ParentProfile { get; set; }

        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}


