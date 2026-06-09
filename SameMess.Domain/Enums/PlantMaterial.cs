namespace SameMess.Domain.Enums;

/// <summary>
/// Nguyên liệu tưới "Cây tình yêu". % lên cây càng cao thì càng HIẾM
/// (nước &lt; nắng &lt; phân bón) để 3 loại đều có ý nghĩa.
/// </summary>
public static class PlantMaterial
{
    public const string Water = "Water";       // 💧 dễ kiếm nhất, +ít %
    public const string Sun = "Sun";           // ☀️ vừa
    public const string Fertilizer = "Fertilizer"; // 🌱 hiếm nhất, +nhiều %

    public static readonly string[] All = { Water, Sun, Fertilizer };

    public static bool IsValid(string material) => All.Contains(material);
}
