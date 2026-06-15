namespace SameMess.Domain.Entities;

/// <summary>Cấu hình hệ thống dạng key-value (admin chỉnh).</summary>
public class AppSetting
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}
