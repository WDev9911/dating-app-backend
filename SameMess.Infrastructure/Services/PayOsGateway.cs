using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SameMess.Application.DTOs.Billing;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Cổng PayOS: tạo payment-request (VietQR) và verify webhook bằng HMAC-SHA256 (ChecksumKey).
/// Chữ ký = HMAC_SHA256( "key1=val1&key2=val2..." sắp xếp khoá a→z ), hex thường.
/// </summary>
public class PayOsGateway : IPayOsGateway
{
    private readonly HttpClient _http;
    private readonly PayOsSettings _s;

    public PayOsGateway(HttpClient http, IOptions<PayOsSettings> settings)
    {
        _http = http;
        _s = settings.Value;
    }

    public async Task<PayOsCreateResultDto> CreatePaymentAsync(long orderCode, int amountVnd, string description)
    {
        // Chữ ký create: đúng thứ tự a→z: amount, cancelUrl, description, orderCode, returnUrl
        var signData =
            $"amount={amountVnd}&cancelUrl={_s.CancelUrl}&description={description}&orderCode={orderCode}&returnUrl={_s.ReturnUrl}";
        var signature = HmacSha256(signData, _s.ChecksumKey);

        var body = new
        {
            orderCode,
            amount = amountVnd,
            description,
            cancelUrl = _s.CancelUrl,
            returnUrl = _s.ReturnUrl,
            signature,
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, $"{_s.BaseUrl.TrimEnd('/')}/v2/payment-requests")
        {
            Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"),
        };
        req.Headers.Add("x-client-id", _s.ClientId);
        req.Headers.Add("x-api-key", _s.ApiKey);

        var resp = await _http.SendAsync(req);
        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var code = root.TryGetProperty("code", out var c) ? c.GetString() : null;
        if (code != "00" || !root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
        {
            var desc = root.TryGetProperty("desc", out var d) ? d.GetString() : "unknown";
            throw new InvalidOperationException($"PayOS tạo đơn thất bại: {code} - {desc}");
        }

        return new PayOsCreateResultDto
        {
            OrderCode = orderCode,
            AmountVnd = amountVnd,
            CheckoutUrl = data.TryGetProperty("checkoutUrl", out var u) ? u.GetString() ?? "" : "",
            QrCode = data.TryGetProperty("qrCode", out var q) ? q.GetString() ?? "" : "",
            PaymentLinkId = data.TryGetProperty("paymentLinkId", out var p) ? p.GetString() ?? "" : "",
        };
    }

    public PayOsWebhookResult VerifyWebhook(string rawJsonBody)
    {
        var result = new PayOsWebhookResult();
        try
        {
            using var doc = JsonDocument.Parse(rawJsonBody);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
                return result;
            var signature = root.TryGetProperty("signature", out var sig) ? sig.GetString() ?? "" : "";

            // Dựng chuỗi ký từ data: sắp xếp khoá a→z, value ép về chuỗi
            var pairs = data.EnumerateObject()
                .OrderBy(x => x.Name, StringComparer.Ordinal)
                .Select(x => $"{x.Name}={ValueToString(x.Value)}");
            var signData = string.Join("&", pairs);
            var expected = HmacSha256(signData, _s.ChecksumKey);

            result.SignatureValid = string.Equals(expected, signature, StringComparison.OrdinalIgnoreCase);

            if (data.TryGetProperty("orderCode", out var oc) && oc.TryGetInt64(out var ocv)) result.OrderCode = ocv;
            if (data.TryGetProperty("amount", out var am) && am.TryGetInt32(out var amv)) result.AmountVnd = amv;

            var code = root.TryGetProperty("code", out var rc) ? rc.GetString() : null;
            var success = root.TryGetProperty("success", out var sc) && sc.ValueKind == JsonValueKind.True;
            result.Success = code == "00" || success;
        }
        catch { /* body không hợp lệ -> coi như không hợp lệ */ }
        return result;
    }

    private static string ValueToString(JsonElement v) => v.ValueKind switch
    {
        JsonValueKind.String => v.GetString() ?? string.Empty,
        JsonValueKind.Null => string.Empty,
        JsonValueKind.Number => v.GetRawText(),
        JsonValueKind.True => "true",
        JsonValueKind.False => "false",
        _ => v.GetRawText(),
    };

    private static string HmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }
}
