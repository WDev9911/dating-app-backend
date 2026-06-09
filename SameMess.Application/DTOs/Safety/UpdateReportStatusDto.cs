namespace SameMess.Application.DTOs.Safety;

public class UpdateReportStatusDto
{
    public string Status { get; set; } = null!; // Reviewed | Resolved | Dismissed
    public bool BanUser { get; set; }            // true => ban luôn người bị báo cáo
}
