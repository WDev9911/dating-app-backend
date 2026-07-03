using SameMess.Application.DTOs.Ai;
using SameMess.Application.DTOs.Chat;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ChatService : IChatService
{
    private const int MaxMessageLength = 2000;

    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAiAssistantService _aiAssistant;
    private readonly ITaskService _taskService;
    private readonly IReputationService _reputationService;
    private readonly INotificationService _notificationService;
    private readonly IVenueRepository _venueRepository;

    public ChatService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IAiAssistantService aiAssistant,
        ITaskService taskService,
        IReputationService reputationService,
        INotificationService notificationService,
        IVenueRepository venueRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _aiAssistant = aiAssistant;
        _taskService = taskService;
        _reputationService = reputationService;
        _notificationService = notificationService;
        _venueRepository = venueRepository;
    }

    public async Task<List<ConversationDto>> GetConversationsAsync(Guid userId)
    {
        var conversations = await _conversationRepository.GetForUserAsync(userId);
        if (conversations.Count == 0)
            return new List<ConversationDto>();

        var otherIds = conversations
            .Select(c => OtherUserId(c.Match, userId))
            .ToList();

        var others = await _userRepository.GetWithProfileAndPhotosByIdsAsync(otherIds);
        var byId = others.ToDictionary(u => u.Id);

        var result = new List<ConversationDto>();
        foreach (var conv in conversations)
        {
            var otherId = OtherUserId(conv.Match, userId);
            byId.TryGetValue(otherId, out var other);

            var lastMessage = await _messageRepository.GetLastAsync(conv.Id);
            var unread = await _messageRepository.CountUnreadAsync(conv.Id, userId);

            result.Add(new ConversationDto
            {
                Id = conv.Id,
                MatchId = conv.MatchId,
                OtherUserId = otherId,
                OtherDisplayName = other?.Profile?.DisplayName ?? string.Empty,
                OtherAvatarUrl = MatchService.AvatarOf(other),
                OtherIsAdmin = other?.Role == UserRole.Admin,
                OtherAvatarFrame = other?.Profile?.AvatarFrame,
                LastMessageText = lastMessage?.Content,
                LastMessageAt = conv.LastMessageAt,
                UnreadCount = unread,
            });
        }

        return result;
    }

    public async Task<ConversationDto> GetOrCreateByMatchAsync(Guid userId, Guid matchId)
    {
        var match = await _matchRepository.GetByIdForUserAsync(matchId, userId)
            ?? throw new NotFoundException("Match", matchId);

        if (!match.IsActive)
            throw new ForbiddenException("This match is no longer active.");

        var conversation = await _conversationRepository.GetByMatchIdAsync(matchId);
        if (conversation is null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                CreatedAt = DateTime.UtcNow,
            };
            await _conversationRepository.AddAsync(conversation);
            await _conversationRepository.SaveChangesAsync();
        }

        var otherId = OtherUserId(match, userId);
        var other = (await _userRepository.GetWithProfileAndPhotosByIdsAsync(new[] { otherId })).FirstOrDefault();

        return new ConversationDto
        {
            Id = conversation.Id,
            MatchId = matchId,
            OtherUserId = otherId,
            OtherDisplayName = other?.Profile?.DisplayName ?? string.Empty,
            OtherAvatarUrl = MatchService.AvatarOf(other),
            OtherIsAdmin = other?.Role == UserRole.Admin,
            OtherAvatarFrame = other?.Profile?.AvatarFrame,
            LastMessageText = null,
            LastMessageAt = conversation.LastMessageAt,
            UnreadCount = 0,
        };
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid userId, Guid conversationId, Guid? beforeMessageId, int limit)
    {
        await EnsureParticipantAsync(userId, conversationId);

        var messages = await _messageRepository.GetPageAsync(conversationId, beforeMessageId, limit);

        // Nạp thông tin quán cho các tin kiểu "venue" để render thẻ
        var venueIds = messages.Where(m => m.VenueId.HasValue).Select(m => m.VenueId!.Value).Distinct().ToList();
        var venues = new Dictionary<Guid, Venue>();
        foreach (var vid in venueIds)
        {
            var v = await _venueRepository.GetByIdAsync(vid);
            if (v is not null) venues[vid] = v;
        }

        return messages
            .Select(m => ToDto(m, m.VenueId.HasValue && venues.TryGetValue(m.VenueId.Value, out var vv) ? vv : null))
            .ToList();
    }

    public async Task<(MessageDto Message, Guid OtherUserId)> ShareVenueAsync(Guid userId, Guid conversationId, Guid venueId)
    {
        var (conversation, match) = await LoadAsync(userId, conversationId);
        if (!match.IsActive)
            throw new ForbiddenException("This match is no longer active.");

        var venue = await _venueRepository.GetByIdAsync(venueId)
            ?? throw new NotFoundException("Venue", venueId);

        var now = DateTime.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = userId,
            Content = venue.Name,                 // preview cho danh sách hội thoại
            Type = MessageType.Venue,
            VenueId = venueId,
            SentAt = now,
        };
        await _messageRepository.AddAsync(message);
        conversation.LastMessageAt = now;
        await _messageRepository.SaveChangesAsync();

        var recipientId = OtherUserId(match, userId);
        try
        {
            await _notificationService.NotifyAsync(recipientId, NotificationType.Message,
                "Gợi ý địa điểm", $"đã chia sẻ địa điểm: {venue.Name}", conversationId.ToString());
        }
        catch { /* không để thông báo làm hỏng luồng chat */ }

        return (ToDto(message, venue), recipientId);
    }

    public async Task<(MessageDto Message, Guid OtherUserId)> SendMessageAsync(Guid userId, Guid conversationId, string content)
    {
        content = (content ?? string.Empty).Trim();
        if (content.Length == 0)
            throw new BadRequestException("Message content is required.");
        if (content.Length > MaxMessageLength)
            throw new BadRequestException($"Message must not exceed {MaxMessageLength} characters.");

        var (conversation, match) = await LoadAsync(userId, conversationId);
        if (!match.IsActive)
            throw new ForbiddenException("This match is no longer active.");

        // Kiểm duyệt nội dung bằng AI trước khi lưu (fail-open: AI lỗi thì vẫn cho gửi)
        ModerationResult moderation;
        try { moderation = await _aiAssistant.ModerateMessageAsync(content); }
        catch { moderation = new ModerationResult { IsFlagged = false }; }
        if (moderation.IsFlagged)
        {
            // Tin bị gắn cờ (nhẹ) → trừ uy tín người gửi (fail-safe)
            try { await _reputationService.RecordEventAsync(userId, ReputationEventType.MessageFlagged); }
            catch { /* không để uy tín làm hỏng luồng chat */ }
            throw new BadRequestException("Tin nhắn vi phạm tiêu chuẩn cộng đồng và đã bị chặn.");
        }

        var now = DateTime.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = userId,
            Content = content,
            SentAt = now,
        };
        await _messageRepository.AddAsync(message);

        conversation.LastMessageAt = now;
        await _messageRepository.SaveChangesAsync();

        // Gamification (fail-safe): tiến độ "gửi tin nhắn"
        try { await _taskService.RecordActionAsync(userId, GameAction.SendMessage); }
        catch { /* không để gamification làm hỏng luồng chat */ }

        // Thông báo cho người nhận (fail-safe). SignalR lo realtime khi online; cái này cho feed + push offline.
        var recipientId = OtherUserId(match, userId);
        try
        {
            var preview = content.Length > 80 ? content[..80] + "…" : content;
            await _notificationService.NotifyAsync(recipientId, NotificationType.Message,
                "Tin nhắn mới", preview, conversationId.ToString());
        }
        catch { /* không để thông báo làm hỏng luồng chat */ }

        return (ToDto(message), recipientId);
    }

    public async Task MarkReadAsync(Guid userId, Guid conversationId)
    {
        await EnsureParticipantAsync(userId, conversationId);
        await _messageRepository.MarkReadAsync(conversationId, userId);
    }

    public async Task<Guid> GetOtherParticipantAsync(Guid userId, Guid conversationId)
    {
        var (_, match) = await LoadAsync(userId, conversationId);
        return OtherUserId(match, userId);
    }

    private async Task<(Conversation Conversation, Match Match)> LoadAsync(Guid userId, Guid conversationId)
    {
        var conversation = await _conversationRepository.GetWithMatchAsync(conversationId)
            ?? throw new NotFoundException("Conversation", conversationId);

        var match = conversation.Match;
        if (match.UserAId != userId && match.UserBId != userId)
            throw new ForbiddenException("You are not a participant of this conversation.");

        return (conversation, match);
    }

    private async Task EnsureParticipantAsync(Guid userId, Guid conversationId) =>
        await LoadAsync(userId, conversationId);

    private static Guid OtherUserId(Match match, Guid userId) =>
        match.UserAId == userId ? match.UserBId : match.UserAId;

    private static MessageDto ToDto(Message m, Venue? venue = null) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        Content = m.Content,
        Type = m.Type,
        SentAt = m.SentAt,
        ReadAt = m.ReadAt,
        VenueId = m.VenueId,
        VenueName = venue?.Name,
        VenueImageUrl = venue?.ImageUrl,
        VenueAddress = venue?.Address,
        VenueCategory = venue?.Category,
    };
}
