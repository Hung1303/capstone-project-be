using Core.Base;

namespace BusinessObjects
{
    public class ApprovalRequest : BaseEntity
    {
        public Guid CourseId { get; set; }
        public ApprovalDecision Decision { get; set; } = ApprovalDecision.Pending;
        public Guid? DecidedByUserId { get; set; }
        public DateTimeOffset? DecidedAt { get; set; }
        public string? Notes { get; set; }

        public virtual Course Course { get; set; } = null!;
        public virtual User? DecidedByUser { get; set; }
    }
}


