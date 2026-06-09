namespace SameMess.Application.DTOs.Billing;

/// <summary>Kết quả parse + verify chữ ký một callback (Return/IPN) từ cổng thanh toán.</summary>
public class PaymentCallbackResult
{
    public bool SignatureValid { get; set; }
    public string TxnRef { get; set; } = string.Empty;
    public string ResponseCode { get; set; } = string.Empty; // "00" = thành công
    public bool IsSuccess => SignatureValid && ResponseCode == "00";
    public string? TransactionNo { get; set; }
    public int AmountVnd { get; set; }
}
