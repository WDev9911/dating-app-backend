using SameMess.Application.DTOs.Billing;

namespace SameMess.Application.Interfaces.Services;

/// <summary>Trừu tượng cổng thanh toán (impl đầu: VNPay) — đổi/ghép cổng khác dễ.</summary>
public interface IPaymentGateway
{
    /// <summary>Tạo URL redirect sang cổng để user thanh toán.</summary>
    string CreatePaymentUrl(string txnRef, int amountVnd, string orderInfo, string clientIp);

    /// <summary>Parse + xác thực chữ ký dữ liệu callback (Return/IPN).</summary>
    PaymentCallbackResult ParseAndVerify(IDictionary<string, string> query);
}
