using SameMess.Application.DTOs.Connection;

namespace SameMess.Application.Interfaces.Services;

public interface IConnectionService
{
    Task<List<ReminderDto>> GetRemindersAsync(Guid userId);
    Task<List<NudgeDto>> GetNudgesAsync(Guid userId, Guid conversationId);
    Task DismissNudgeAsync(Guid userId, Guid conversationId, string nudgeId);
    Task<MeetupResultDto> ProposeMeetupAsync(Guid userId, Guid conversationId, ProposeMeetupDto dto);
}
