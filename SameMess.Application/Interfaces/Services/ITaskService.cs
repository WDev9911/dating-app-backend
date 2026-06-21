using SameMess.Application.DTOs.Gamification;

namespace SameMess.Application.Interfaces.Services;

public interface ITaskService
{
    /// <summary>
    /// Ghi nhận một hành vi (GameAction) → cộng tiến độ mọi nhiệm vụ tương ứng,
    /// hoàn thành thì cộng nguyên liệu vào kho. Gọi từ các service sẵn có (fail-safe).
    /// </summary>
    Task RecordActionAsync(Guid userId, string action, int amount = 1);

    /// <summary>Danh sách nhiệm vụ + tiến độ kỳ hiện tại (đồng thời tính là check-in hôm nay).</summary>
    Task<List<TaskDto>> GetTasksAsync(Guid userId);

    Task<List<InventoryItemDto>> GetInventoryAsync(Guid userId);

    /// <summary>Nhận thưởng cho một nhiệm vụ đã hoàn thành (cộng nguyên liệu, đánh dấu Claimed).</summary>
    Task ClaimAsync(Guid userId, string taskCode);
}
