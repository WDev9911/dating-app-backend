using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IReportRepository : IBaseRepository<Report>
{
    /// <summary>Lấy report theo trạng thái (null = tất cả), mới nhất trước.</summary>
    Task<List<Report>> GetByStatusAsync(string? status);
}
