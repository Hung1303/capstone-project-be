using Core.Base;

namespace BusinessObjects.DTO.AuditLog
{
    public class AuditLogListItem
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTimeOffset OccurredAt { get; set; }
    }

    public class AuditLogDetail
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public AuditActionType ActionType { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuditLogListRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? UserId { get; set; }
        public AuditActionType? ActionType { get; set; }
        public string? EntityName { get; set; }
        public Guid? EntityId { get; set; }
        public DateTimeOffset? From { get; set; }
        public DateTimeOffset? To { get; set; }
        public string? Search { get; set; }
    }
}


