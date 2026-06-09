using SameMess.Application.DTOs.Safety;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;

    public ReportService(IReportRepository reportRepository, IUserRepository userRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
    }

    public async Task ReportAsync(Guid userId, Guid targetUserId, ReportRequestDto dto)
    {
        if (targetUserId == userId)
            throw new BadRequestException("You cannot report yourself.");

        _ = await _userRepository.GetByIdAsync(targetUserId)
            ?? throw new NotFoundException("User", targetUserId);

        await _reportRepository.AddAsync(new Report
        {
            Id = Guid.NewGuid(),
            ReporterId = userId,
            ReportedId = targetUserId,
            Reason = dto.Reason,
            Description = dto.Description?.Trim(),
            Status = ReportStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        });

        await _reportRepository.SaveChangesAsync();
    }
}
