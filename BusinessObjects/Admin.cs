using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Admin : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; } = null!;

        // Business Properties
        [Required]
        [MaxLength(100)]
        public string EmployeeId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Department { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        public bool CanApproveTeachers { get; set; } = false;

        public bool CanApproveClasses { get; set; } = false;

        public bool CanViewReports { get; set; } = true;

        public bool CanManageUsers { get; set; } = false;

        // Collection Navigation Properties
        public virtual ICollection<TeacherApproval> TeacherApprovals { get; set; } = new List<TeacherApproval>();
        public virtual ICollection<ClassApproval> ClassApprovals { get; set; } = new List<ClassApproval>();
    }
}
