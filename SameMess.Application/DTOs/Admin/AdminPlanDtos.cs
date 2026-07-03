namespace SameMess.Application.DTOs.Admin;

public class PlanPayloadDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int PriceVnd { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AdminPlanDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int PriceVnd { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; }
}

public class SubscriberDto
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string PlanCode { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

public class GrantPlanPayloadDto
{
    public Guid UserId { get; set; }
    public string PlanCode { get; set; } = null!;
}
