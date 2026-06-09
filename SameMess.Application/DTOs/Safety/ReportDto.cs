namespace SameMess.Application.DTOs.Safety;

public class ReportDto
{
    public Guid Id { get; set; }
    public Guid ReportedId { get; set; }
    public string ReportedDisplayName { get; set; } = string.Empty;
    public Guid ReporterId { get; set; }
    public string Reason { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
