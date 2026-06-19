using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IMeetupProposalRepository : IBaseRepository<MeetupProposal>
{
    /// <summary>Đề xuất đang chờ (Proposed) của một hội thoại — mỗi cặp tối đa 1.</summary>
    Task<MeetupProposal?> GetPendingByConversationAsync(Guid conversationId);

    /// <summary>Toàn bộ đề xuất của hội thoại (mới nhất trước).</summary>
    Task<List<MeetupProposal>> GetByConversationAsync(Guid conversationId);
}
