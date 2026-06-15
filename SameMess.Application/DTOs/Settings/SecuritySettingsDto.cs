namespace SameMess.Application.DTOs.Settings;

public class SecuritySettingsDto
{
    public string Email { get; set; } = null!;
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public bool LoginAlertsEnabled { get; set; }
}
