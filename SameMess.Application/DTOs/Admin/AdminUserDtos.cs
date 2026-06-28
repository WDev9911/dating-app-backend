namespace SameMess.Application.DTOs.Admin;

public class AdminUserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string Role { get; set; } = null!;
    public string Status { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public bool IsPhotoVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastActiveAt { get; set; }   // mốc hoạt động gần nhất (UTC)
    public bool IsOnline { get; set; }             // còn hoạt động trong vài phút gần đây
}

public class AdminUserDetailDto : AdminUserListItemDto
{
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string VerificationStatus { get; set; } = null!;
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateUserStatusDto
{
    public string Status { get; set; } = null!;
}

public class AddNoteDto
{
    public string Content { get; set; } = null!;
}

public class AdminNoteDto
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class AdminResetPasswordDto
{
    public string NewPassword { get; set; } = null!;
}

public class BulkActionDto
{
    public string Action { get; set; } = null!; // "ban" | "unban" | "delete-sessions"
    public List<Guid> UserIds { get; set; } = new();
}

public class BulkActionResultDto
{
    public int Affected { get; set; }
}
