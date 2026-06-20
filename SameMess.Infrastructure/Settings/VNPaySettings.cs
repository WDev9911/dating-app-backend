namespace SameMess.Infrastructure.Settings;

public class VNPaySettings
{
    public string TmnCode { get; set; } = string.Empty;   // vnp_TmnCode (mã website merchant)
    public string HashSecret { get; set; } = string.Empty; // bí mật ký HMAC-SHA512
    public string BaseUrl { get; set; } = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    public string ReturnUrl { get; set; } = "https://localhost:7097/api/payments/vnpay/return";
    public string FrontendReturnUrl { get; set; } = "http://localhost:5173/premium"; // chuyển người dùng về web sau thanh toán
    public string Version { get; set; } = "2.1.0";
    public string Command { get; set; } = "pay";
    public string CurrCode { get; set; } = "VND";
    public string Locale { get; set; } = "vn";
}
