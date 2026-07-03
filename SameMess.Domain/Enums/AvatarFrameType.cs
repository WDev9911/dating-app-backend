namespace SameMess.Domain.Enums;

/// <summary>Khung hiệu ứng động quanh avatar — hiện tại chỉ Admin được đổi (huy hiệu uy lực).</summary>
public static class AvatarFrameType
{
    public const string Fire = "Fire";
    public const string Ice = "Ice";
    public const string Gold = "Gold";
    public const string Electric = "Electric";

    public static readonly string[] All = { Fire, Ice, Gold, Electric };

    public static bool IsValid(string? frame) => frame is null || All.Contains(frame);
}
