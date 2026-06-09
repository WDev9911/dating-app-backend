using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminReportService
{
    Task<List<ReportDto>> GetReportsAsync(string? status);
    Task ResolveAsync(Guid reportId, UpdateReportStatusDto dto);
}
