namespace SameMess.Application.DTOs.Billing;

public class SubscriptionDto
{
    public string PlanCode { get; set; } = null!;
    public bool IsActive { get; set; }           // có gói trả phí còn hạn không
    public DateTime? ExpiresAt { get; set; }
    public EntitlementsDto Entitlements { get; set; } = new();
}
