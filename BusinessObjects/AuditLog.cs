using Core.Base;

namespace BusinessObjects
{
    public class AuditLog : BaseEntity
    {
        public Guid UserId { get; set; }
        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    }
}


