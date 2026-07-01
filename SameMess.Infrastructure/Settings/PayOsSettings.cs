namespace SameMess.Infrastructure.Settings;

public class PayOsSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChecksumKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api-merchant.payos.vn";
    // Người dùng được PayOS chuyển về sau khi thanh toán / hủy (trang web)
    public string ReturnUrl { get; set; } = "http://localhost:5173/premium?payment=success";
    public string CancelUrl { get; set; } = "http://localhost:5173/premium?payment=cancel";

    // Gốc URL frontend — dùng dựng link trang voucher công khai (QR quét ra) và
    // return/cancel cho luồng Date Pass. Ưu tiên set qua env PayOS__FrontendBaseUrl.
    public string FrontendBaseUrl { get; set; } = "https://exe-dating.vercel.app";
}
