namespace SameMess.Domain.Entities;

public class OtpCode
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Purpose { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;
}
