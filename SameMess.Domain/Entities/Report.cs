namespace SameMess.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public Guid ReportedId { get; set; }
    public string Reason { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
