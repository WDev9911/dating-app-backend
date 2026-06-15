namespace SameMess.Application.DTOs.Safety;

public class SafetySettingsDto
{
    public bool PinEnabled { get; set; }
    public bool EmergencyAlertEnabled { get; set; }
    public bool CheckinEnabled { get; set; }
}

public class UpdateSafetySettingsDto
{
    public bool? PinEnabled { get; set; }
    public bool? EmergencyAlertEnabled { get; set; }
    public bool? CheckinEnabled { get; set; }
}
