using SameMess.Application.DTOs.Safety;

namespace SameMess.Application.Interfaces.Services;

public interface IReportService
{
    Task ReportAsync(Guid userId, Guid targetUserId, ReportRequestDto dto);
}
