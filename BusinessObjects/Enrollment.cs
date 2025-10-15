using Core.Base;

namespace BusinessObjects
{
    public class Enrollment : BaseEntity
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }
        public Guid StudentProfileId { get; set; }
        public virtual StudentProfile StudentProfile { get; set; }
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
        public DateTimeOffset? ConfirmedAt { get; set; }
        public DateTimeOffset? CancelledAt { get; set; }
        public string? CancelReason { get; set; }
    }
}


