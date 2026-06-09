namespace SameMess.Application.DTOs.Safety;

public class ReportRequestDto
{
    public string Reason { get; set; } = null!;
    public string? Description { get; set; }
}
