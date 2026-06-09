namespace SameMess.Application.DTOs.Profile;

public class PhotoDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public int OrderIndex { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}
