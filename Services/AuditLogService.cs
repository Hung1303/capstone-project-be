using BusinessObjects;
using BusinessObjects.DTO.AuditLog;
using Core.Base;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<AuditLogListItem> Items, int TotalCount)> GetAuditLogsAsync(AuditLogListRequest request)
        {
            var logs = _unitOfWork.GetRepository<AuditLog>().Entities;
            var users = _unitOfWork.GetRepository<User>().Entities;

            var query = from l in logs
                        join u in users on l.UserId equals u.Id
                        where !u.IsDeleted
                        select new { l, u };

            if (request.UserId.HasValue)
                query = query.Where(x => x.l.UserId == request.UserId.Value);

            if (request.ActionType.HasValue)
                query = query.Where(x => x.l.ActionType == request.ActionType.Value);

            if (!string.IsNullOrWhiteSpace(request.EntityName))
                query = query.Where(x => x.l.EntityName == request.EntityName);

            if (request.EntityId.HasValue)
                query = query.Where(x => x.l.EntityId == request.EntityId.Value);

            if (request.From.HasValue)
                query = query.Where(x => x.l.OccurredAt >= request.From.Value);

            if (request.To.HasValue)
                query = query.Where(x => x.l.OccurredAt <= request.To.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = $"%{request.Search}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.u.FullName, keyword) ||
                    EF.Functions.Like(x.l.EntityName, keyword) ||
                    EF.Functions.Like(x.l.Details, keyword) ||
                    EF.Functions.Like(x.l.IpAddress, keyword));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.l.OccurredAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AuditLogListItem
                {
                    Id = x.l.Id,
                    UserId = x.l.UserId,
                    UserFullName = x.u.FullName,
                    ActionType = x.l.ActionType,
                    EntityName = x.l.EntityName,
                    EntityId = x.l.EntityId,
                    IpAddress = x.l.IpAddress,
                    OccurredAt = x.l.OccurredAt
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<AuditLogDetail?> GetAuditLogByIdAsync(Guid id)
        {
            var logs = _unitOfWork.GetRepository<AuditLog>().Entities;
            var users = _unitOfWork.GetRepository<User>().Entities;

            var query = from l in logs
                        join u in users on l.UserId equals u.Id
                        where l.Id == id
                        select new AuditLogDetail
                        {
                            Id = l.Id,
                            UserId = l.UserId,
                            UserFullName = u.FullName,
                            UserEmail = u.Email,
                            ActionType = l.ActionType,
                            EntityName = l.EntityName,
                            EntityId = l.EntityId,
                            Details = l.Details,
                            IpAddress = l.IpAddress,
                            UserAgent = l.UserAgent,
                            OccurredAt = l.OccurredAt,
                            CreatedAt = l.CreatedAt
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task LogAsync(
            Guid userId,
            AuditActionType actionType,
            string entityName,
            Guid? entityId,
            string details,
            string ipAddress,
            string? userAgent = null,
            DateTimeOffset? occurredAt = null)
        {
            var repo = _unitOfWork.GetRepository<AuditLog>();
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                UserId = userId,
                ActionType = actionType,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                OccurredAt = occurredAt ?? DateTimeOffset.UtcNow
            };
            await repo.InsertAsync(log);
            await _unitOfWork.SaveAsync();
        }
    }
}


