using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IDailyQuestCompletionRepository : IBaseRepository<DailyQuestCompletion>
{
    Task<List<DailyQuestCompletion>> GetByUserAndPeriodAsync(Guid userId, string periodKey);
}
