namespace SameMess.Application.DTOs.Settings;

public class UpdateSecuritySettingsDto
{
    public bool? TwoFactorEnabled { get; set; }
    public bool? LoginAlertsEnabled { get; set; }
}
