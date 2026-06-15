namespace SameMess.Application.DTOs.Safety;

public class SetupPinDto
{
    public string Pin { get; set; } = null!;
}

public class ForgotPinDto
{
    public string Channel { get; set; } = "email"; // "email" | "sms"
}

public class VerifyPinOtpDto
{
    public string Otp { get; set; } = null!;
}
