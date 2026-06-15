using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface INudgeDismissalRepository : IBaseRepository<NudgeDismissal>
{
    Task<List<string>> GetCodesAsync(Guid userId, Guid conversationId);
}
