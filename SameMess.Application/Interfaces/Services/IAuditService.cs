using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Common;

namespace SameMess.Application.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(Guid adminId, string action, string? targetType = null, Guid? targetId = null, string? details = null);
    Task<PagedResultDto<AuditLogDto>> GetLogsAsync(string? action, Guid? adminId, int page, int pageSize);
}
