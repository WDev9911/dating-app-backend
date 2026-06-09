using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtp, ILogger<EmailService> logger)
    {
        _smtp = smtp.Value;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode, string purpose)
    {
        var (subject, heading, subheading) = purpose switch
        {
            "EmailVerification" => (
                "Verify your SameMess account",
                "Verify Your Email",
                "Use the code below to verify your email address and activate your account."),
            "PasswordReset" => (
                "Reset your SameMess password",
                "Reset Your Password",
                "Use the code below to reset your password. If you didn't request this, you can safely ignore this email."),
            _ => (
                "Your SameMess OTP Code",
                "Your Verification Code",
                "Use the code below to complete your action.")
        };

        var digits = otpCode.Select(c => $@"
            <span style=""
                display: inline-block;
                width: 48px;
                height: 64px;
                line-height: 64px;
                text-align: center;
                font-size: 32px;
                font-weight: 700;
                color: #ff4d6d;
                background: #fff0f3;
                border: 2px solid #ffb3c1;
                border-radius: 12px;
                margin: 0 4px;
                font-family: 'Courier New', monospace;
            "">{c}</span>").ToList();

        var otpHtml = string.Join("", digits);

        var body = $@"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  <title>{subject}</title>
</head>
<body style=""margin:0; padding:0; background:#f4f6fb; font-family: 'Segoe UI', Arial, sans-serif;"">

  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f4f6fb; padding: 40px 16px;"">
    <tr>
      <td align=""center"">
        <table width=""100%"" cellpadding=""0"" cellspacing=""0""
               style=""max-width:560px; background:#ffffff; border-radius:20px;
                       box-shadow: 0 4px 24px rgba(0,0,0,0.08); overflow:hidden;"">

          <!-- Header -->
          <tr>
            <td style=""background: linear-gradient(135deg, #ff4d6d 0%, #c9184a 100%);
                        padding: 40px 40px 32px; text-align:center;"">
              <div style=""display:inline-flex; align-items:center; gap:10px;"">
                <span style=""font-size:28px;"">💬</span>
                <span style=""font-size:26px; font-weight:800; color:#ffffff;
                              letter-spacing:-0.5px;"">SameMess</span>
              </div>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding: 40px 40px 32px;"">

              <h1 style=""margin:0 0 8px; font-size:22px; font-weight:700; color:#1a1a2e;"">
                {heading}
              </h1>
              <p style=""margin:0 0 32px; font-size:15px; color:#6b7280; line-height:1.6;"">
                {subheading}
              </p>

              <!-- OTP Box -->
              <div style=""background:#fff8f9; border:1.5px solid #ffe0e6; border-radius:16px;
                           padding:28px 24px; text-align:center; margin-bottom:28px;"">
                <p style=""margin:0 0 16px; font-size:12px; font-weight:600;
                           color:#9ca3af; letter-spacing:2px; text-transform:uppercase;"">
                  Verification Code
                </p>
                <div style=""margin-bottom:16px;"">
                  {otpHtml}
                </div>
                <p style=""margin:0; font-size:13px; color:#9ca3af;"">
                  ⏱ Expires in <strong style=""color:#ff4d6d;"">10 minutes</strong>
                </p>
              </div>

              <!-- Warning -->
              <div style=""background:#fffbeb; border-left:4px solid #f59e0b;
                           border-radius:8px; padding:14px 16px; margin-bottom:28px;"">
                <p style=""margin:0; font-size:13px; color:#92400e; line-height:1.5;"">
                  🔒 <strong>Never share this code</strong> with anyone.
                  SameMess will never ask for your OTP via phone or chat.
                </p>
              </div>

              <p style=""margin:0; font-size:13px; color:#9ca3af; line-height:1.6;"">
                Didn't request this? You can safely ignore this email.
                Your account remains secure.
              </p>

            </td>
          </tr>

          <!-- Divider -->
          <tr>
            <td style=""padding: 0 40px;"">
              <hr style=""border:none; border-top:1px solid #f3f4f6; margin:0;"" />
            </td>
          </tr>

          <!-- Footer -->
          <tr>
            <td style=""padding: 24px 40px 32px; text-align:center;"">
              <p style=""margin:0 0 6px; font-size:13px; color:#9ca3af;"">
                Sent by <strong style=""color:#ff4d6d;"">SameMess</strong> — Connect, Match, Chat
              </p>
              <p style=""margin:0; font-size:11px; color:#d1d5db;"">
                © {DateTime.UtcNow.Year} SameMess. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";

        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
        };

        var message = new MailMessage
        {
            From = new MailAddress(_smtp.FromEmail, _smtp.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        message.To.Add(toEmail);

        await client.SendMailAsync(message);
        _logger.LogInformation("OTP email sent to {Email} for purpose {Purpose}", toEmail, purpose);
    }
}
