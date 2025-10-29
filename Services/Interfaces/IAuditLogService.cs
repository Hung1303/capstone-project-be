using Services.DTO.AuditLog;

namespace Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<(IEnumerable<AuditLogListItem> Items, int TotalCount)> GetAuditLogsAsync(AuditLogListRequest request);
        Task<AuditLogDetail?> GetAuditLogByIdAsync(Guid id);
        Task LogAsync(Guid userId, Core.Base.AuditActionType actionType, string entityName, Guid? entityId, string details, string ipAddress, string? userAgent = null, DateTimeOffset? occurredAt = null);
    }
}


