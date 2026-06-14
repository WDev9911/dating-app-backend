using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Gửi email qua Resend HTTP API (https://api.resend.com/emails) — hoạt động trên cloud
/// vì dùng HTTPS (cổng 443), không bị chặn như SMTP. Được chọn khi có Resend:ApiKey.
/// </summary>
public class ResendEmailService : IEmailService
{
    private readonly HttpClient _http;
    private readonly ResendSettings _settings;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(HttpClient http, IOptions<ResendSettings> settings, ILogger<ResendEmailService> logger)
    {
        _http = http;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode, string purpose)
    {
        var (subject, html) = OtpEmailTemplate.Build(otpCode, purpose);

        var payload = new
        {
            from = $"{_settings.FromName} <{_settings.FromEmail}>",
            to = new[] { toEmail },
            subject,
            html,
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails")
        {
            Content = JsonContent.Create(payload),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Resend API trả về {(int)response.StatusCode}: {detail}");
        }

        _logger.LogInformation("OTP email sent via Resend to {Email} for purpose {Purpose}", toEmail, purpose);
    }
}
