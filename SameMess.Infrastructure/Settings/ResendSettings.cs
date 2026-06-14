namespace SameMess.Infrastructure.Settings;

public class ResendSettings
{
    public string ApiKey { get; set; } = "";
    // Khi chưa verify domain trên Resend: dùng "onboarding@resend.dev" (chỉ gửi tới email tài khoản bạn).
    // Đã verify domain: đổi thành "noreply@your-domain.com" để gửi tới mọi người.
    public string FromEmail { get; set; } = "onboarding@resend.dev";
    public string FromName { get; set; } = "SameMess";
}
