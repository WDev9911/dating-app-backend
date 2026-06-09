namespace SameMess.Application.Interfaces.Services;

public interface IIcebreakerService
{
    /// <summary>Gợi ý câu mở lời cho một match (xác thực user là thành viên + match còn active).</summary>
    Task<List<string>> SuggestForMatchAsync(Guid userId, Guid matchId);
}
