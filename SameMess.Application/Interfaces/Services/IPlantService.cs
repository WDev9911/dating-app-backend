using SameMess.Application.DTOs.Gamification;

namespace SameMess.Application.Interfaces.Services;

public interface IPlantService
{
    /// <summary>Xem cây chung của một match (tạo mới nếu chưa có).</summary>
    Task<PlantDto> GetPlantAsync(Guid userId, Guid matchId);

    /// <summary>Tưới cây bằng một nguyên liệu: tiêu kho → cộng % → check level-up + streak.</summary>
    Task<WaterResultDto> WaterAsync(Guid userId, Guid matchId, string material);
}
