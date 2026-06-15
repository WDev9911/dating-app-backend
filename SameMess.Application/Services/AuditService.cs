using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Common;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditRepository;

    public AuditService(IAuditLogRepository auditRepository) => _auditRepository = auditRepository;

    public async Task LogAsync(Guid adminId, string action, string? targetType = null, Guid? targetId = null, string? details = null)
    {
        await _auditRepository.AddAsync(new AuditLog
        {
            Id = Guid.NewGuid(),
            AdminId = adminId,
            Action = action,
            TargetType = targetType,
            TargetId = targetId,
            Details = details,
            CreatedAt = DateTime.UtcNow,
        });
        await _auditRepository.SaveChangesAsync();
    }

    public async Task<PagedResultDto<AuditLogDto>> GetLogsAsync(string? action, Guid? adminId, int page, int pageSize)
    {
        var (items, total) = await _auditRepository.SearchAsync(action, adminId, page, pageSize);
        return new PagedResultDto<AuditLogDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = items.Select(a => new AuditLogDto
            {
                Id = a.Id,
                AdminId = a.AdminId,
                Action = a.Action,
                TargetType = a.TargetType,
                TargetId = a.TargetId,
                Details = a.Details,
                CreatedAt = a.CreatedAt,
            }).ToList(),
        };
    }
}
