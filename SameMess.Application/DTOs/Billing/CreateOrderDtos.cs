namespace SameMess.Application.DTOs.Billing;

public class CreateOrderRequestDto
{
    /// <summary>Gói muốn mua: Plus / Gold.</summary>
    public string PlanCode { get; set; } = null!;
}

public class CreateOrderResultDto
{
    public string TxnRef { get; set; } = null!;
    public int AmountVnd { get; set; }
    public string PaymentUrl { get; set; } = null!; // redirect user sang VNPay
}
