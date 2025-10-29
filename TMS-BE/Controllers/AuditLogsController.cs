using Services.DTO.AuditLog;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] Guid? userId = null,
            [FromQuery] int? actionType = null,
            [FromQuery] string? entityName = null,
            [FromQuery] Guid? entityId = null,
            [FromQuery] DateTimeOffset? from = null,
            [FromQuery] DateTimeOffset? to = null,
            [FromQuery] string? search = null)
        {
            var request = new AuditLogListRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                UserId = userId,
                ActionType = actionType.HasValue ? (Core.Base.AuditActionType?)actionType.Value : null,
                EntityName = entityName,
                EntityId = entityId,
                From = from,
                To = to,
                Search = search
            };

            var (items, totalCount) = await _auditLogService.GetAuditLogsAsync(request);
            return Ok(new { totalCount, items });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAuditLogById(Guid id)
        {
            var log = await _auditLogService.GetAuditLogByIdAsync(id);
            if (log == null)
                return NotFound(new { message = "Audit log not found" });
            return Ok(log);
        }
    }
}


