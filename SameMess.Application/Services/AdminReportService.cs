using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminReportService : IAdminReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReputationService _reputationService;

    public AdminReportService(
        IReportRepository reportRepository,
        IUserRepository userRepository,
        IReputationService reputationService)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _reputationService = reputationService;
    }

    public async Task<List<ReportDto>> GetReportsAsync(string? status)
    {
        var reports = await _reportRepository.GetByStatusAsync(status);
        if (reports.Count == 0)
            return new List<ReportDto>();

        var reportedIds = reports.Select(r => r.ReportedId).Distinct().ToList();
        var users = await _userRepository.GetWithProfileByIdsAsync(reportedIds);
        var byId = users.ToDictionary(u => u.Id);

        return reports.Select(r => new ReportDto
        {
            Id = r.Id,
            ReportedId = r.ReportedId,
            ReportedDisplayName = byId.TryGetValue(r.ReportedId, out var u) ? (u.Profile?.DisplayName ?? string.Empty) : string.Empty,
            ReporterId = r.ReporterId,
            Reason = r.Reason,
            Description = r.Description,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
        }).ToList();
    }

    public async Task ResolveAsync(Guid reportId, UpdateReportStatusDto dto)
    {
        var report = await _reportRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException("Report", reportId);

        report.Status = dto.Status;
        await _reportRepository.UpdateAsync(report);

        // Tùy chọn: ban luôn người bị báo cáo
        if (dto.BanUser)
        {
            var user = await _userRepository.GetByIdAsync(report.ReportedId);
            if (user is not null)
            {
                user.Status = UserStatus.Banned;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }

        await _reportRepository.SaveChangesAsync();

        // Report được admin xử lý (Resolved) = vi phạm nặng đã xác minh → trừ uy tín người bị báo cáo
        if (dto.Status == ReportStatus.Resolved)
        {
            try
            {
                await _reputationService.RecordEventAsync(
                    report.ReportedId, ReputationEventType.ReportUpheld, $"Report {report.Reason} resolved by admin");
            }
            catch { /* không để uy tín làm hỏng luồng admin */ }
        }
    }
}
