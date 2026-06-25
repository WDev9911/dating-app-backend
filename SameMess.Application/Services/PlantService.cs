using SameMess.Application.DTOs.Gamification;
using SameMess.Application.Gamification;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class PlantService : IPlantService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchPlantRepository _plantRepository;
    private readonly IUserInventoryRepository _inventoryRepository;
    private readonly ITaskService _taskService;

    public PlantService(
        IMatchRepository matchRepository,
        IMatchPlantRepository plantRepository,
        IUserInventoryRepository inventoryRepository,
        ITaskService taskService)
    {
        _matchRepository = matchRepository;
        _plantRepository = plantRepository;
        _inventoryRepository = inventoryRepository;
        _taskService = taskService;
    }

    public async Task<PlantDto> GetPlantAsync(Guid userId, Guid matchId)
    {
        var match = await EnsureActiveMatchAsync(userId, matchId);
        var (plant, created) = await GetOrCreatePlantAsync(matchId);
        if (created)
            await _plantRepository.SaveChangesAsync();

        return ToDto(plant, match, userId);
    }

    public async Task<WaterResultDto> WaterAsync(Guid userId, Guid matchId, string material)
    {
        if (!PlantMaterial.IsValid(material))
            throw new BadRequestException("Nguyên liệu không hợp lệ.");

        var match = await EnsureActiveMatchAsync(userId, matchId);

        // Phải còn nguyên liệu trong kho
        var stock = await _inventoryRepository.GetAsync(userId, material);
        if (stock is null || stock.Quantity < 1)
            throw new BadRequestException($"Bạn không còn {material} để tưới.");

        var (plant, _) = await GetOrCreatePlantAsync(matchId);

        // Cây đã đạt cấp tối đa → khoá, không cho tưới thêm (tránh phí nguyên liệu)
        if (plant.Level >= GamificationConfig.MaxLevel)
            throw new BadRequestException($"Cây đã đạt cấp tối đa (Cấp {GamificationConfig.MaxLevel}) 🌳💖");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var isUserA = match.UserAId == userId;

        // Reset cờ "đã tưới hôm nay" khi sang ngày mới
        if (plant.WaterDate != today)
        {
            plant.WaterDate = today;
            plant.WateredByA = false;
            plant.WateredByB = false;
        }

        // Chuỗi chỉ tính một lần/ngày, vào lần tưới ĐẦU TIÊN trong ngày (bất kể ai tưới)
        if (plant.StreakDate != today)
        {
            AdvanceStreak(plant, today);
            plant.StreakDate = today;
        }

        // Đánh dấu ai vừa tưới + xác định có phải lần "chốt cặp" để thưởng bonus không
        var bothBefore = plant.WateredByA && plant.WateredByB;
        if (isUserA) plant.WateredByA = true;
        else plant.WateredByB = true;
        var bothNow = plant.WateredByA && plant.WateredByB;
        var bonusApplied = bothNow && !bothBefore;

        // Cộng % (nhân đôi nếu là lần chốt cả hai cùng tưới)
        var growth = GamificationConfig.GrowthPercent(material)
            * (bonusApplied ? GamificationConfig.BothWateredBonusMultiplier : 1);

        plant.GrowthPercent += growth;
        var leveledUp = false;
        var milestoneReached = false;
        while (plant.GrowthPercent >= GamificationConfig.PercentPerLevel
               && plant.Level < GamificationConfig.MaxLevel)
        {
            plant.GrowthPercent -= GamificationConfig.PercentPerLevel;
            plant.Level += 1;
            leveledUp = true;
            if (GamificationConfig.IsMilestone(plant.Level))
                milestoneReached = true;
        }

        // Đạt cấp tối đa (7): khoá cấp, giữ thanh tiến độ đầy 100%, không tích luỹ % thừa
        if (plant.Level >= GamificationConfig.MaxLevel)
        {
            plant.Level = GamificationConfig.MaxLevel;
            plant.GrowthPercent = GamificationConfig.PercentPerLevel;
        }

        // Tiêu nguyên liệu
        stock.Quantity -= 1;
        plant.UpdatedAt = DateTime.UtcNow;

        await _plantRepository.SaveChangesAsync();

        // Ghi nhận hành vi tưới cho nhiệm vụ (fail-safe, không để hỏng việc tưới)
        try { await _taskService.RecordActionAsync(userId, GameAction.Water); }
        catch { /* gamification không được làm gián đoạn luồng chính */ }

        return new WaterResultDto
        {
            Level = plant.Level,
            GrowthPercent = plant.GrowthPercent,
            StreakCount = plant.StreakCount,
            LeveledUp = leveledUp,
            BonusApplied = bonusApplied,
            MilestoneReached = milestoneReached,
            Message = BuildMessage(leveledUp, milestoneReached, bonusApplied, plant.Level),
        };
    }

    /// <summary>Tính lại chuỗi cho lần tưới đầu tiên trong ngày. Có "đóng băng" tha 1 ngày/tuần.</summary>
    private static void AdvanceStreak(MatchPlant plant, DateOnly today)
    {
        if (plant.StreakDate is null)
        {
            plant.StreakCount = 1;
            return;
        }

        var gap = today.DayNumber - plant.StreakDate.Value.DayNumber;
        if (gap == 1)
        {
            plant.StreakCount += 1;
            return;
        }

        // Quên đúng 1 ngày → thử dùng quyền đóng băng tuần này
        var weekKey = GamificationConfig.WeekKey(today);
        if (gap == 1 + GamificationConfig.FreezeGraceDays && plant.FreezeUsedWeekKey != weekKey)
        {
            plant.FreezeUsedWeekKey = weekKey;
            plant.StreakCount += 1; // tha, giữ chuỗi
        }
        else
        {
            plant.StreakCount = 1; // đứt chuỗi
        }
    }

    private async Task<Match> EnsureActiveMatchAsync(Guid userId, Guid matchId)
    {
        var match = await _matchRepository.GetByIdForUserAsync(matchId, userId)
            ?? throw new NotFoundException("Match", matchId);
        if (!match.IsActive)
            throw new ForbiddenException("Match này không còn hoạt động.");
        return match;
    }

    private async Task<(MatchPlant Plant, bool Created)> GetOrCreatePlantAsync(Guid matchId)
    {
        var plant = await _plantRepository.GetByMatchIdAsync(matchId);
        if (plant is not null)
            return (plant, false);

        plant = new MatchPlant
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            Level = 1,
            GrowthPercent = 0,
            StreakCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await _plantRepository.AddAsync(plant);
        return (plant, true);
    }

    private static PlantDto ToDto(MatchPlant plant, Match match, Guid userId)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var freshDay = plant.WaterDate == today;
        var wateredByA = freshDay && plant.WateredByA;
        var wateredByB = freshDay && plant.WateredByB;
        var iWatered = match.UserAId == userId ? wateredByA : wateredByB;

        return new PlantDto
        {
            MatchId = plant.MatchId,
            Level = plant.Level,
            GrowthPercent = plant.GrowthPercent,
            PercentPerLevel = GamificationConfig.PercentPerLevel,
            StreakCount = plant.StreakCount,
            IWateredToday = iWatered,
            BothWateredToday = wateredByA && wateredByB,
        };
    }

    private static string BuildMessage(bool leveledUp, bool milestone, bool bonus, int level)
    {
        if (milestone)
            return $"🎉 Cây đạt mốc level {level}! Cặp đôi nhận huy hiệu kỷ niệm.";
        if (leveledUp)
            return $"🌱 Cây lớn lên level {level}!";
        if (bonus)
            return "💞 Cả hai cùng tưới hôm nay — thưởng nhân đôi!";
        return "💧 Đã tưới cây.";
    }
}
