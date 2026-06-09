namespace SameMess.Domain.Entities;

/// <summary>
/// Kho nguyên liệu của một user (riêng từng người), mang đi tưới cây chung.
/// Mỗi (UserId, MaterialType) chỉ một dòng (unique).
/// </summary>
public class UserInventory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MaterialType { get; set; } = null!; // PlantMaterial
    public int Quantity { get; set; }
}
