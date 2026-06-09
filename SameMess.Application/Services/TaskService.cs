using SameMess.Application.DTOs.Gamification;
using SameMess.Application.Gamification;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUserTaskProgressRepository _progressRepository;
    private readonly IUserInventoryRepository _inventoryRepository;

    public TaskService(
        IUserTaskProgressRepository progressRepository,
        IUserInventoryRepository inventoryRepository)
    {
        _progressRepository = progressRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task RecordActionAsync(Guid userId, string action, int amount = 1)
    {
        if (amount <= 0) return;

        var tasks = GamificationConfig.Tasks.Where(t => t.Action == action).ToList();
        if (tasks.Count == 0) return;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var changed = false;

        foreach (var task in tasks)
        {
            var periodKey = GamificationConfig.PeriodKey(task.Type, today);
            var progress = await _progressRepository.GetAsync(userId, task.Code, periodKey);

            if (progress is null)
            {
                progress = new UserTaskProgress
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TaskCode = task.Code,
                    PeriodKey = periodKey,
                    Progress = 0,
                    Completed = false,
                };
                await _progressRepository.AddAsync(progress);
            }

            if (progress.Completed) continue;

            progress.Progress += amount;
            changed = true;

            if (progress.Progress >= task.Target)
            {
                progress.Progress = task.Target;
                progress.Completed = true;
                progress.CompletedAt = DateTime.UtcNow;
                await GrantMaterialAsync(userId, task.RewardMaterial, task.RewardQty);
            }
        }

        if (changed)
            await _progressRepository.SaveChangesAsync();
    }

    public async Task<List<TaskDto>> GetTasksAsync(Guid userId)
    {
        // Mở danh sách nhiệm vụ tính là check-in hôm nay
        await RecordActionAsync(userId, GameAction.Login);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var keys = GamificationConfig.Tasks
            .Select(t => GamificationConfig.PeriodKey(t.Type, today))
            .Distinct()
            .ToList();

        var rows = await _progressRepository.GetByUserAndKeysAsync(userId, keys);
        var byKey = rows.ToDictionary(r => (r.TaskCode, r.PeriodKey));

        return GamificationConfig.Tasks.Select(t =>
        {
            var periodKey = GamificationConfig.PeriodKey(t.Type, today);
            byKey.TryGetValue((t.Code, periodKey), out var p);
            return new TaskDto
            {
                Code = t.Code,
                Type = t.Type,
                Description = t.Description,
                Target = t.Target,
                Progress = p?.Progress ?? 0,
                Completed = p?.Completed ?? false,
                RewardMaterial = t.RewardMaterial,
                RewardQty = t.RewardQty,
            };
        }).ToList();
    }

    public async Task<List<InventoryItemDto>> GetInventoryAsync(Guid userId)
    {
        var rows = await _inventoryRepository.GetByUserAsync(userId);
        var byType = rows.ToDictionary(r => r.MaterialType, r => r.Quantity);

        // Trả đủ 3 loại (zero-fill) để client luôn hiển thị đủ
        return PlantMaterial.All
            .Select(m => new InventoryItemDto
            {
                Material = m,
                Quantity = byType.TryGetValue(m, out var q) ? q : 0,
            })
            .ToList();
    }

    /// <summary>Cộng nguyên liệu vào kho (cùng DbContext nên SaveChanges của progress sẽ persist chung).</summary>
    private async Task GrantMaterialAsync(Guid userId, string material, int quantity)
    {
        var item = await _inventoryRepository.GetAsync(userId, material);
        if (item is null)
        {
            await _inventoryRepository.AddAsync(new UserInventory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MaterialType = material,
                Quantity = quantity,
            });
        }
        else
        {
            item.Quantity += quantity;
        }
    }
}
