using Core.Base;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interfaces;
using System.Security.Claims;

namespace API.Filters
{
    public class AuditLogActionFilter : IAsyncActionFilter
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogActionFilter(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var method = httpContext.Request.Method?.ToUpperInvariant();

            // Only log write operations
            var shouldLog = method != "GET";
            var userId = GetUserId(httpContext.User);

            // Proceed with the action
            var executedContext = await next();

            if (!shouldLog || userId == null)
                return;

            try
            {
                var route = httpContext.Request.Path.Value ?? string.Empty;
                var entityName = context.Controller.GetType().Name.Replace("Controller", string.Empty);
                var entityId = TryExtractGuidFromRoute(executedContext.RouteData?.Values);
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();

                var actionType = MapMethodToActionType(method);
                var details = $"{method} {route}";

                await _auditLogService.LogAsync(
                    userId.Value,
                    actionType,
                    entityName,
                    entityId,
                    details,
                    ip,
                    userAgent);
            }
            catch
            {
                // Swallow logging errors to not affect main flow
            }
        }

        private static Guid? GetUserId(ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst("UserId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var id)) return id;
            return null;
        }

        private static Guid? TryExtractGuidFromRoute(IDictionary<string, object>? routeValues)
        {
            if (routeValues == null) return null;
            foreach (var kv in routeValues)
            {
                if (kv.Value is string s && Guid.TryParse(s, out var gid)) return gid;
            }
            return null;
        }

        private static AuditActionType MapMethodToActionType(string? method)
        {
            return method switch
            {
                "POST" => AuditActionType.Create,
                "PUT" => AuditActionType.Update,
                "PATCH" => AuditActionType.Update,
                "DELETE" => AuditActionType.Delete,
                _ => AuditActionType.Update
            };
        }
    }
}


