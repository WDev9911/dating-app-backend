namespace SameMess.Application.DTOs.Admin;

// ===== Interests =====
public class AdminInterestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? GroupName { get; set; }
    public bool IsActive { get; set; }
}

public class InterestPayloadDto
{
    public string Name { get; set; } = null!;
    public string? GroupName { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateInterestGroupDto
{
    public string Name { get; set; } = null!;
}

// ===== Settings / Feature flags =====
public class AdminSettingsDto
{
    public Dictionary<string, string> Settings { get; set; } = new();
    public Dictionary<string, bool> FeatureFlags { get; set; } = new();
}

public class UpdateSettingsDto
{
    public Dictionary<string, string> Settings { get; set; } = new();
}

public class UpdateFeatureFlagDto
{
    public bool Value { get; set; }
}
