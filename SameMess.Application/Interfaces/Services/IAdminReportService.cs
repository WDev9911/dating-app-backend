using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminReportService
{
    Task<List<ReportDto>> GetReportsAsync(string? status);
    Task ResolveAsync(Guid reportId, UpdateReportStatusDto dto);

    Task<ReportDto> GetReportDetailAsync(Guid reportId);
    Task AssignAsync(Guid reportId, Guid assignToAdminId);
    Task ResolveReportAsync(Guid reportId, string? action, string? note);
    Task DismissReportAsync(Guid reportId, string? note);
}
