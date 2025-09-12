using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class Attendance : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string StudentId { get; set; }
        public virtual Student Student { get; set; } = null!;

        [Required]
        public string ClassSessionId { get; set; }
        public virtual ClassSession ClassSession { get; set; } = null!;

        [Required]
        public string EnrollmentId { get; set; }
        public virtual Enrollment Enrollment { get; set; } = null!;

        // Business Properties
        public bool IsPresent { get; set; } = false;

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? AbsenceReason { get; set; }

        public bool IsExcused { get; set; } = false;
    }
}
