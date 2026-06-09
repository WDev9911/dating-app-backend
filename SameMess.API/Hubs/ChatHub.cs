using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Exceptions;

namespace SameMess.API.Hubs;

/// <summary>
/// Hub chat realtime. Client kết nối tới /hubs/chat (kèm access_token qua query string).
/// Tin nhắn được bắn tới cả hai thành viên qua Clients.User (định danh bằng claim "sub").
/// </summary>
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService) => _chatService = chatService;

    private Guid UserId
    {
        get
        {
            var sub = Context.User?.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var id))
                throw new UnauthorizedException("Invalid token claims.");
            return id;
        }
    }

    /// <summary>Gửi tin nhắn; lưu DB rồi bắn "ReceiveMessage" cho cả người gửi và người nhận.</summary>
    public async Task SendMessage(Guid conversationId, string content)
    {
        var (message, otherUserId) = await _chatService.SendMessageAsync(UserId, conversationId, content);

        await Clients.Users(UserId.ToString(), otherUserId.ToString())
            .SendAsync("ReceiveMessage", message);
    }

    /// <summary>Thông báo "đang gõ" cho người còn lại.</summary>
    public async Task Typing(Guid conversationId)
    {
        var otherUserId = await _chatService.GetOtherParticipantAsync(UserId, conversationId);
        await Clients.User(otherUserId.ToString())
            .SendAsync("UserTyping", conversationId, UserId);
    }

    /// <summary>Đánh dấu đã đọc và báo cho người gửi biết tin đã được đọc.</summary>
    public async Task MarkRead(Guid conversationId)
    {
        await _chatService.MarkReadAsync(UserId, conversationId);

        var otherUserId = await _chatService.GetOtherParticipantAsync(UserId, conversationId);
        await Clients.User(otherUserId.ToString())
            .SendAsync("MessagesRead", conversationId, UserId);
    }
}
