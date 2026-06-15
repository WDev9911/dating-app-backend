namespace SameMess.Domain.Entities;

public class Photo
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Url { get; set; } = null!;
    public int OrderIndex { get; set; }
    public bool IsPrimary { get; set; }
    public string Status { get; set; } = Enums.PhotoStatus.Approved; // mặc định duyệt để không chặn luồng hiện tại
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
