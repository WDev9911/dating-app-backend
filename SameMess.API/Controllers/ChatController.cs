using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SameMess.API.Extensions;
using SameMess.Application.DTOs.Chat;
using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Controllers;

[Authorize]
[Route("api/conversations")]
public class ChatController : ApiControllerBase
{
    private const int DefaultMessageLimit = 30;
    private const int MaxMessageLimit = 100;

    private readonly IChatService _chatService;
    private readonly IValidator<SendMessageDto> _sendMessageValidator;

    public ChatController(IChatService chatService, IValidator<SendMessageDto> sendMessageValidator)
    {
        _chatService = chatService;
        _sendMessageValidator = sendMessageValidator;
    }

    /// <summary>Danh sách hội thoại của tôi (kèm tin nhắn cuối + số chưa đọc).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations()
        => Ok(await _chatService.GetConversationsAsync(CurrentUserId));

    /// <summary>Lấy (hoặc tạo) hội thoại cho một match — dùng để bắt đầu chat.</summary>
    [HttpPost("by-match/{matchId:guid}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrCreateByMatch(Guid matchId)
        => Ok(await _chatService.GetOrCreateByMatchAsync(CurrentUserId, matchId));

    /// <summary>Lịch sử tin nhắn (phân trang: truyền before = id tin cũ nhất đang có để lấy trang cũ hơn).</summary>
    [HttpGet("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] Guid? before, [FromQuery] int limit = DefaultMessageLimit)
    {
        limit = Math.Clamp(limit, 1, MaxMessageLimit);
        return Ok(await _chatService.GetMessagesAsync(CurrentUserId, conversationId, before, limit));
    }

    /// <summary>Gửi tin nhắn (REST fallback — realtime nên dùng qua SignalR hub).</summary>
    [HttpPost("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageDto dto)
    {
        var validation = await _sendMessageValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToValidationProblemDetails());

        var (message, _) = await _chatService.SendMessageAsync(CurrentUserId, conversationId, dto.Content);
        return Ok(message);
    }

    /// <summary>Đánh dấu đã đọc toàn bộ tin trong hội thoại.</summary>
    [HttpPost("{conversationId:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkRead(Guid conversationId)
    {
        await _chatService.MarkReadAsync(CurrentUserId, conversationId);
        return NoContent();
    }
}
