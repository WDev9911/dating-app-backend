namespace SameMess.Application.DTOs.Billing;

public class PlanDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int PriceVnd { get; set; }
    public int DurationDays { get; set; }
}
