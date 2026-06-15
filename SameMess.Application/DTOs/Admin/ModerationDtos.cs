namespace SameMess.Application.DTOs.Admin;

// ===== Reports =====
public class AssignReportDto
{
    public Guid AdminId { get; set; }
}

public class ResolveReportDto
{
    public string? Action { get; set; }  // null | "ban" (ban người bị báo cáo)
    public string? Note { get; set; }
}

public class DismissReportDto
{
    public string? Note { get; set; }
}

// ===== Verifications =====
public class ApproveVerificationDto
{
    public string? Note { get; set; }
}

public class RejectVerificationDto
{
    public string? Reason { get; set; }
    public string? Note { get; set; }
}

// ===== Photos =====
public class AdminPhotoDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? DisplayName { get; set; }
    public string Url { get; set; } = null!;
    public string Status { get; set; } = null!;
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RejectPhotoDto
{
    public string? Reason { get; set; }
}
