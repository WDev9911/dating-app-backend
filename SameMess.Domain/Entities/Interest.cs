namespace SameMess.Domain.Entities;

/// <summary>Danh mục sở thích (catalog) — quản trị qua admin, user chọn để gắn vào hồ sơ.</summary>
public class Interest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? GroupName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
}
