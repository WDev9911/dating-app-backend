using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SameMess.Application.DTOs.Billing;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Cổng VNPay (chuẩn 2.1.0): build payment URL + ký/verify HMAC-SHA512.
/// Tham số sắp xếp a→z (ordinal) + URL-encode đồng nhất giữa lúc ký và lúc verify.
/// </summary>
public class VNPayGateway : IPaymentGateway
{
    private readonly VNPaySettings _settings;

    public VNPayGateway(IOptions<VNPaySettings> settings) => _settings = settings.Value;

    public string CreatePaymentUrl(string txnRef, int amountVnd, string orderInfo, string clientIp)
    {
        // Giờ VN (GMT+7) để khớp vnp_CreateDate/vnp_ExpireDate
        var vnNow = DateTime.UtcNow.AddHours(7);

        var data = new SortedList<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = _settings.Version,
            ["vnp_Command"] = _settings.Command,
            ["vnp_TmnCode"] = _settings.TmnCode,
            ["vnp_Amount"] = (amountVnd * 100L).ToString(CultureInfo.InvariantCulture), // VNPay nhân 100
            ["vnp_CurrCode"] = _settings.CurrCode,
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = orderInfo,
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = _settings.Locale,
            ["vnp_ReturnUrl"] = _settings.ReturnUrl,
            ["vnp_IpAddr"] = NormalizeIp(clientIp),
            ["vnp_CreateDate"] = vnNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            ["vnp_ExpireDate"] = vnNow.AddMinutes(15).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
        };

        // Chuẩn VNPay 2.1.0: ký HMAC-SHA512 trên CHÍNH chuỗi query đã url-encode (key=value đã sort)
        var hashData = BuildEncodedQuery(data);
        var secureHash = HmacSha512(_settings.HashSecret, hashData);
        return $"{_settings.BaseUrl}?{hashData}&vnp_SecureHash={secureHash}";
    }

    public PaymentCallbackResult ParseAndVerify(IDictionary<string, string> query)
    {
        var received = query.TryGetValue("vnp_SecureHash", out var h) ? h : string.Empty;

        // Lấy mọi tham số vnp_* trừ chữ ký, sắp xếp ordinal, encode đồng nhất rồi ký lại để so
        var data = new SortedList<string, string>(StringComparer.Ordinal);
        foreach (var kv in query)
        {
            if (kv.Key.StartsWith("vnp_", StringComparison.Ordinal)
                && kv.Key != "vnp_SecureHash"
                && kv.Key != "vnp_SecureHashType")
            {
                data[kv.Key] = kv.Value;
            }
        }

        // Verify: ký lại trên chuỗi query đã url-encode (đồng nhất với lúc tạo)
        var hashData = BuildEncodedQuery(data);
        var computed = HmacSha512(_settings.HashSecret, hashData);
        var valid = !string.IsNullOrEmpty(received)
                    && string.Equals(computed, received, StringComparison.OrdinalIgnoreCase);

        query.TryGetValue("vnp_Amount", out var amountRaw);
        var amount = long.TryParse(amountRaw, out var a) ? (int)(a / 100) : 0;

        return new PaymentCallbackResult
        {
            SignatureValid = valid,
            TxnRef = query.TryGetValue("vnp_TxnRef", out var t) ? t : string.Empty,
            ResponseCode = query.TryGetValue("vnp_ResponseCode", out var rc) ? rc : string.Empty,
            TransactionNo = query.TryGetValue("vnp_TransactionNo", out var tn) ? tn : null,
            AmountVnd = amount,
        };
    }

    /// <summary>Chuỗi query đã url-encode (WebUtility = hex chữ HOA, space → +), key đã sort — dùng cho cả URL lẫn ký.</summary>
    private static string BuildEncodedQuery(SortedList<string, string> data)
    {
        var sb = new StringBuilder();
        foreach (var kv in data)
        {
            if (string.IsNullOrEmpty(kv.Value)) continue;
            if (sb.Length > 0) sb.Append('&');
            sb.Append(WebUtility.UrlEncode(kv.Key)).Append('=').Append(WebUtility.UrlEncode(kv.Value));
        }
        return sb.ToString();
    }

    /// <summary>IPv6 loopback (::1) → IPv4 cho gọn; trống → 127.0.0.1.</summary>
    private static string NormalizeIp(string? ip) =>
        string.IsNullOrWhiteSpace(ip) || ip == "::1" ? "127.0.0.1" : ip;

    private static string HmacSha512(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder(hash.Length * 2);
        foreach (var b in hash) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
        return sb.ToString();
    }
}
