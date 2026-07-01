namespace SameMess.Application.DTOs.Billing;

/// <summary>Kết quả tạo link thanh toán PayOS trả về cho frontend.</summary>
public class PayOsCreateResultDto
{
    public long OrderCode { get; set; }
    public int AmountVnd { get; set; }
    public string CheckoutUrl { get; set; } = string.Empty; // link quét/chuyển hướng
    public string QrCode { get; set; } = string.Empty;      // chuỗi VietQR để render QR
    public string PaymentLinkId { get; set; } = string.Empty;
}

/// <summary>Kết quả verify + parse webhook PayOS (server→server).</summary>
public class PayOsWebhookResult
{
    public bool SignatureValid { get; set; }
    public bool Success { get; set; }   // code == "00"
    public long OrderCode { get; set; }
    public int AmountVnd { get; set; }
}
