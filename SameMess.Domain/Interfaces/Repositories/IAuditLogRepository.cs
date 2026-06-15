using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IAuditLogRepository : IBaseRepository<AuditLog>
{
    Task<(List<AuditLog> Items, int Total)> SearchAsync(string? action, Guid? adminId, int page, int pageSize);
}
