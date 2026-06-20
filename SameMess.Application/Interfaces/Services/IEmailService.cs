namespace SameMess.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otpCode, string purpose);

    /// <summary>Gửi hóa đơn + voucher (kèm QR) cho combo hẹn hò.</summary>
    Task SendVoucherEmailAsync(string toEmail, VoucherEmailModel model);
}

/// <summary>Dữ liệu để dựng email voucher.</summary>
public class VoucherEmailModel
{
    public string VenueName { get; set; } = null!;
    public string ComboTitle { get; set; } = null!;
    public int AmountVnd { get; set; }
    public string VoucherCode { get; set; } = null!;
    public string QrUrl { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
