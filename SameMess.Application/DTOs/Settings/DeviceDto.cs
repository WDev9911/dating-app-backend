namespace SameMess.Application.DTOs.Settings;

public class DeviceDto
{
    public Guid Id { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
