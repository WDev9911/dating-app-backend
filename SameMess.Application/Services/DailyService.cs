using SameMess.Application.Daily;
using SameMess.Application.DTOs.Daily;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class DailyService : IDailyService
{
    private readonly IDailyQuestCompletionRepository _completionRepository;
    private readonly IUserXpRepository _xpRepository;

    public DailyService(
        IDailyQuestCompletionRepository completionRepository,
        IUserXpRepository xpRepository)
    {
        _completionRepository = completionRepository;
        _xpRepository = xpRepository;
    }

    public async Task<DailyConnectionDto> GetConnectionAsync(Guid userId)
    {
        var period = DailyQuestCatalog.TodayPeriodKey();
        var done = (await _completionRepository.GetByUserAndPeriodAsync(userId, period))
            .Select(c => c.QuestCode).ToHashSet();
        var userXp = (await _xpRepository.GetByUserAsync(userId))?.TotalXp ?? 0;

        return new DailyConnectionDto
        {
            Quests = DailyQuestCatalog.All.Select(q => new DailyQuestDto
            {
                Code = q.Code,
                Title = q.Title,
                Description = q.Description,
                XpReward = q.XpReward,
                Completed = done.Contains(q.Code),
            }).ToList(),
            TotalXp = DailyQuestCatalog.TotalXp,
            UserXp = userXp,
        };
    }

    public async Task<CompleteDailyResultDto> CompleteAsync(Guid userId, CompleteDailyDto dto)
    {
        var period = DailyQuestCatalog.TodayPeriodKey();
        var alreadyDone = (await _completionRepository.GetByUserAndPeriodAsync(userId, period))
            .Select(c => c.QuestCode).ToHashSet();

        var completed = new List<string>();
        var xpEarned = 0;

        foreach (var code in dto.QuestIds.Distinct())
        {
            var quest = DailyQuestCatalog.Find(code);
            if (quest is null || alreadyDone.Contains(code)) continue;   // bỏ mã sai / đã hoàn thành hôm nay

            await _completionRepository.AddAsync(new DailyQuestCompletion
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                QuestCode = code,
                PeriodKey = period,
                XpAwarded = quest.XpReward,
                CompletedAt = DateTime.UtcNow,
            });
            completed.Add(code);
            xpEarned += quest.XpReward;
        }

        if (completed.Count > 0)
            await _completionRepository.SaveChangesAsync();

        var xp = await _xpRepository.GetByUserAsync(userId);
        if (xp is null)
        {
            xp = new UserXp { Id = Guid.NewGuid(), UserId = userId, TotalXp = xpEarned, UpdatedAt = DateTime.UtcNow };
            await _xpRepository.AddAsync(xp);
        }
        else
        {
            xp.TotalXp += xpEarned;
            xp.UpdatedAt = DateTime.UtcNow;
            await _xpRepository.UpdateAsync(xp);
        }
        await _xpRepository.SaveChangesAsync();

        return new CompleteDailyResultDto { Completed = completed, XpEarned = xpEarned, TotalXp = xp.TotalXp };
    }
}
