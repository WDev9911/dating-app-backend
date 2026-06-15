namespace SameMess.Domain.Entities;

/// <summary>Cờ bật/tắt tính năng theo key (admin chỉnh).</summary>
public class FeatureFlag
{
    public string Key { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public DateTime UpdatedAt { get; set; }
}
