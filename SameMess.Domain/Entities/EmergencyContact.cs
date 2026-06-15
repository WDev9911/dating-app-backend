namespace SameMess.Domain.Entities;

/// <summary>Liên hệ khẩn cấp của user (n).</summary>
public class EmergencyContact
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Relationship { get; set; }
    public DateTime CreatedAt { get; set; }
}
