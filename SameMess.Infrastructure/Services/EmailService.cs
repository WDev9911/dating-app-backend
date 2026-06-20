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
        var (subject, body) = OtpEmailTemplate.Build(otpCode, purpose);
        await SendAsync(toEmail, subject, body);
        _logger.LogInformation("OTP email sent to {Email} for purpose {Purpose}", toEmail, purpose);
    }

    public async Task SendVoucherEmailAsync(string toEmail, VoucherEmailModel model)
    {
        var (subject, body) = VoucherEmailTemplate.Build(model);
        await SendAsync(toEmail, subject, body);
        _logger.LogInformation("Voucher email sent to {Email} (voucher {Code})", toEmail, model.VoucherCode);
    }

    private async Task SendAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtp.Host, _smtp.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
            Timeout = 15000,
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
    }
}
