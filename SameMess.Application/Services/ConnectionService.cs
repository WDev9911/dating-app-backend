using SameMess.Application.DTOs.Connection;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ConnectionService : IConnectionService
{
    private const int QuietHours = 24;
    private const int ReconnectHours = 48;

    private readonly IMatchRepository _matchRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly INudgeDismissalRepository _nudgeRepository;
    private readonly IMeetupProposalRepository _meetupRepository;

    public ConnectionService(
        IMatchRepository matchRepository,
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        INudgeDismissalRepository nudgeRepository,
        IMeetupProposalRepository meetupRepository)
    {
        _matchRepository = matchRepository;
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _nudgeRepository = nudgeRepository;
        _meetupRepository = meetupRepository;
    }

    public async Task<List<ReminderDto>> GetRemindersAsync(Guid userId)
    {
        var matches = await _matchRepository.GetActiveForUserAsync(userId);
        var partnerIds = matches.Select(m => Partner(m, userId)).Distinct().ToList();
        var users = await _userRepository.GetWithProfileByIdsAsync(partnerIds);
        var nameById = users.ToDictionary(u => u.Id, u => u.Profile?.DisplayName);

        var now = DateTime.UtcNow;
        var reminders = new List<ReminderDto>();

        foreach (var m in matches)
        {
            var partnerId = Partner(m, userId);
            var name = nameById.TryGetValue(partnerId, out var n) ? n : null;
            var label = name ?? "đối phương";
            var conv = await _conversationRepository.GetByMatchIdAsync(m.Id);

            if (conv is null || conv.LastMessageAt is null)
            {
                reminders.Add(new ReminderDto
                {
                    Type = "say_hi",
                    MatchId = m.Id,
                    ConversationId = conv?.Id,
                    PartnerId = partnerId,
                    PartnerName = name,
                    Message = $"Bạn đã match với {label} — gửi lời chào đi!",
                });
            }
            else if (conv.LastMessageAt < now.AddHours(-ReconnectHours))
            {
                var days = Math.Max(1, (int)(now - conv.LastMessageAt.Value).TotalDays);
                reminders.Add(new ReminderDto
                {
                    Type = "reconnect",
                    MatchId = m.Id,
                    ConversationId = conv.Id,
                    PartnerId = partnerId,
                    PartnerName = name,
                    Message = $"Cuộc trò chuyện với {label} đã lặng {days} ngày — nhắn lại nhé!",
                    LastActivityAt = conv.LastMessageAt,
                });
            }
        }

        return reminders;
    }

    public async Task<List<NudgeDto>> GetNudgesAsync(Guid userId, Guid conversationId)
    {
        var conv = await EnsureMemberAsync(conversationId, userId);
        var dismissed = (await _nudgeRepository.GetCodesAsync(userId, conversationId)).ToHashSet();
        var now = DateTime.UtcNow;
        var nudges = new List<NudgeDto>();

        void Add(string code, string message)
        {
            if (!dismissed.Contains(code))
                nudges.Add(new NudgeDto { Id = code, ConversationId = conversationId, Message = message });
        }

        if (conv.LastMessageAt is null)
            Add("start", "Hãy gửi lời chào đầu tiên để mở đầu câu chuyện!");
        else if (conv.LastMessageAt < now.AddHours(-QuietHours))
            Add("reengage", "Đã hơn 1 ngày chưa nhắn — thử hỏi xem hôm nay họ thế nào?");

        Add("ask-interest", "Thử hỏi về sở thích chung để hiểu nhau hơn.");
        return nudges;
    }

    public async Task DismissNudgeAsync(Guid userId, Guid conversationId, string nudgeId)
    {
        await EnsureMemberAsync(conversationId, userId);

        var existing = await _nudgeRepository.GetCodesAsync(userId, conversationId);
        if (existing.Contains(nudgeId)) return;

        await _nudgeRepository.AddAsync(new NudgeDismissal
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ConversationId = conversationId,
            NudgeCode = nudgeId,
            CreatedAt = DateTime.UtcNow,
        });
        await _nudgeRepository.SaveChangesAsync();
    }

    public async Task<MeetupResultDto> ProposeMeetupAsync(Guid userId, Guid conversationId, ProposeMeetupDto dto)
    {
        await EnsureMemberAsync(conversationId, userId);

        var proposal = new MeetupProposal
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            ProposerId = userId,
            VenueId = dto.VenueId,
            ProposedAt = dto.ProposedAt,
            Note = dto.Note,
            Status = MeetupStatus.Proposed,
            CreatedAt = DateTime.UtcNow,
        };
        await _meetupRepository.AddAsync(proposal);
        await _meetupRepository.SaveChangesAsync();

        return new MeetupResultDto { MeetupId = proposal.Id, Status = proposal.Status };
    }

    private static Guid Partner(Match m, Guid userId) => m.UserAId == userId ? m.UserBId : m.UserAId;

    private async Task<Conversation> EnsureMemberAsync(Guid conversationId, Guid userId)
    {
        var conv = await _conversationRepository.GetWithMatchAsync(conversationId)
            ?? throw new NotFoundException("Conversation", conversationId);

        if (conv.Match.UserAId != userId && conv.Match.UserBId != userId)
            throw new ForbiddenException("Bạn không thuộc hội thoại này.");

        return conv;
    }
}
