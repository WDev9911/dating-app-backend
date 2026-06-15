namespace SameMess.Application.DTOs.Safety;

public class EmergencyContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Relationship { get; set; }
}

public class EmergencyContactInputDto
{
    public string Name { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Relationship { get; set; }
}

public class EmergencyDto
{
    public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
    public string? AlertMessage { get; set; }
}

/// <summary>Cập nhật liên hệ khẩn cấp + tin nhắn báo động (thay toàn bộ danh sách).</summary>
public class UpsertEmergencyDto
{
    public string? AlertMessage { get; set; }
    public List<EmergencyContactInputDto> Contacts { get; set; } = new();
}
