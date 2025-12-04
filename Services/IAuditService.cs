using DMS.Models;

namespace DMS.Services;

public interface IAuditService
{
    Task LogAsync(string userId, string action, string entityType, int? entityId, string? details);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(int skip = 0, int take = 50);
    Task<IEnumerable<AuditLog>> GetAuditLogsByUserAsync(string userId);
    Task<IEnumerable<AuditLog>> GetAuditLogsByEntityAsync(string entityType, int entityId);
}
