using Core.Base;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class ClassSession : BaseEntity
    {
        // Foreign Keys and Navigation Properties
        [Required]
        public string TutoringClassId { get; set; }
        public virtual TutoringClass TutoringClass { get; set; } = null!;

        //[Required]
        //public string ScheduleId { get; set; }
        //public virtual Schedule Schedule { get; set; } = null!;

        // Business Properties
        [Required]
        public DateTime SessionDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        [MaxLength(200)]
        public string? Room { get; set; }

        [MaxLength(1000)]
        public string? Topic { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(2000)]
        public string? Homework { get; set; }

        public bool IsCompleted { get; set; } = false;

        public bool IsCancelled { get; set; } = false;

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        //public int? ActualDuration { get; set; } // in minutes

        // Collection Navigation Properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
