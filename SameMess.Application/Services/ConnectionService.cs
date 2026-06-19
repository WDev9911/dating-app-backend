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
    private readonly IMatchPlantRepository _plantRepository;
    private readonly IVenueRepository _venueRepository;

    public ConnectionService(
        IMatchRepository matchRepository,
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        INudgeDismissalRepository nudgeRepository,
        IMeetupProposalRepository meetupRepository,
        IMatchPlantRepository plantRepository,
        IVenueRepository venueRepository)
    {
        _matchRepository = matchRepository;
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _nudgeRepository = nudgeRepository;
        _meetupRepository = meetupRepository;
        _plantRepository = plantRepository;
        _venueRepository = venueRepository;
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
                    Type = "say_hi", MatchId = m.Id, ConversationId = conv?.Id,
                    PartnerId = partnerId, PartnerName = name,
                    Message = $"Bạn đã match với {label} — gửi lời chào đi!",
                });
            }
            else if (conv.LastMessageAt < now.AddHours(-ReconnectHours))
            {
                var days = Math.Max(1, (int)(now - conv.LastMessageAt.Value).TotalDays);
                reminders.Add(new ReminderDto
                {
                    Type = "reconnect", MatchId = m.Id, ConversationId = conv.Id,
                    PartnerId = partnerId, PartnerName = name,
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
            Id = Guid.NewGuid(), UserId = userId, ConversationId = conversationId,
            NudgeCode = nudgeId, CreatedAt = DateTime.UtcNow,
        });
        await _nudgeRepository.SaveChangesAsync();
    }

    public async Task<MeetupResultDto> ProposeMeetupAsync(Guid userId, Guid conversationId, ProposeMeetupDto dto)
    {
        var conv = await EnsureMemberAsync(conversationId, userId);

        // Gate: cây của cặp phải đạt Level >= 4
        var plant = await _plantRepository.GetByMatchIdAsync(conv.MatchId);
        if (plant is null || plant.Level < VenueService.UnlockLevel)
            throw new ForbiddenException($"Chăm cây tình yêu đạt Level {VenueService.UnlockLevel} để mở khóa hẹn hò.");

        // Venue (nếu có) phải tồn tại
        if (!string.IsNullOrWhiteSpace(dto.VenueId))
        {
            if (!Guid.TryParse(dto.VenueId, out var vid) || await _venueRepository.GetByIdAsync(vid) is null)
                throw new BadRequestException("Địa điểm không hợp lệ.");
        }

        // Mỗi cặp chỉ 1 đề xuất đang chờ — đề xuất cũ (nếu có) bị thay thế
        var pending = await _meetupRepository.GetPendingByConversationAsync(conversationId);
        if (pending is not null)
        {
            pending.Status = MeetupStatus.Declined;
            await _meetupRepository.UpdateAsync(pending);
        }

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

    public async Task<MeetupDto> RespondMeetupAsync(Guid userId, Guid meetupId, RespondMeetupDto dto)
    {
        var proposal = await _meetupRepository.GetByIdAsync(meetupId)
            ?? throw new NotFoundException("Meetup", meetupId);

        await EnsureMemberAsync(proposal.ConversationId, userId);

        if (proposal.ProposerId == userId)
            throw new BadRequestException("Bạn là người đề xuất, không thể tự phản hồi. Đợi đối phương trả lời.");
        if (proposal.Status != MeetupStatus.Proposed)
            throw new BadRequestException("Đề xuất này đã được xử lý.");

        proposal.Status = dto.Action?.ToLowerInvariant() switch
        {
            "accept" => MeetupStatus.Accepted,
            "decline" => MeetupStatus.Declined,
            _ => throw new BadRequestException("Action phải là 'accept' hoặc 'decline'."),
        };
        await _meetupRepository.UpdateAsync(proposal);
        await _meetupRepository.SaveChangesAsync();

        return await ToDtoAsync(proposal, userId);
    }

    public async Task<List<MeetupDto>> GetMeetupsAsync(Guid userId, Guid conversationId)
    {
        await EnsureMemberAsync(conversationId, userId);
        var proposals = await _meetupRepository.GetByConversationAsync(conversationId);

        var result = new List<MeetupDto>();
        foreach (var p in proposals)
            result.Add(await ToDtoAsync(p, userId));
        return result;
    }

    private async Task<MeetupDto> ToDtoAsync(MeetupProposal p, Guid userId)
    {
        Venue? venue = null;
        if (!string.IsNullOrWhiteSpace(p.VenueId) && Guid.TryParse(p.VenueId, out var vid))
            venue = await _venueRepository.GetByIdAsync(vid);

        return new MeetupDto
        {
            Id = p.Id,
            ConversationId = p.ConversationId,
            ProposerId = p.ProposerId,
            IsMine = p.ProposerId == userId,
            VenueId = venue?.Id,
            VenueName = venue?.Name,
            ProposedAt = p.ProposedAt,
            Note = p.Note,
            Status = p.Status,
            CreatedAt = p.CreatedAt,
        };
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
