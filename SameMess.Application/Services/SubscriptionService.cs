using System.Globalization;
using SameMess.Application.Billing;
using SameMess.Application.DTOs.Billing;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPaymentOrderRepository _orderRepository;
    private readonly IPaymentGateway _gateway;
    private readonly IPayOsGateway _payos;

    public SubscriptionService(
        IPlanRepository planRepository,
        ISubscriptionRepository subscriptionRepository,
        IPaymentOrderRepository orderRepository,
        IPaymentGateway gateway,
        IPayOsGateway payos)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _orderRepository = orderRepository;
        _gateway = gateway;
        _payos = payos;
    }

    public async Task<List<PlanDto>> GetPlansAsync()
    {
        var plans = await _planRepository.GetActiveAsync();
        return plans.Select(p => new PlanDto
        {
            Code = p.Code,
            Name = p.Name,
            PriceVnd = p.PriceVnd,
            DurationDays = p.DurationDays,
        }).ToList();
    }

    public async Task<SubscriptionDto> GetMySubscriptionAsync(Guid userId)
    {
        var sub = await _subscriptionRepository.GetByUserAsync(userId);
        var active = sub is not null && sub.ExpiresAt > DateTime.UtcNow;
        var planCode = active ? sub!.PlanCode : PlanCode.Free;
        var ent = Entitlements.For(planCode);

        return new SubscriptionDto
        {
            PlanCode = planCode,
            IsActive = active,
            ExpiresAt = active ? sub!.ExpiresAt : null,
            Entitlements = ToDto(ent),
        };
    }

    public async Task<PlanEntitlements> GetEntitlementsAsync(Guid userId)
    {
        var sub = await _subscriptionRepository.GetByUserAsync(userId);
        var planCode = sub is not null && sub.ExpiresAt > DateTime.UtcNow ? sub.PlanCode : PlanCode.Free;
        return Entitlements.For(planCode);
    }

    public async Task<CreateOrderResultDto> CreateOrderAsync(Guid userId, string planCode, string clientIp)
    {
        if (!PlanCode.IsValidPaid(planCode))
            throw new BadRequestException("Chỉ mua được gói Plus hoặc Gold.");

        var plan = await _planRepository.GetByCodeAsync(planCode);
        if (plan is null || !plan.IsActive)
            throw new NotFoundException("Plan", planCode);

        var txnRef = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

        var order = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TxnRef = txnRef,
            PlanCode = plan.Code,
            AmountVnd = plan.PriceVnd,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        var url = _gateway.CreatePaymentUrl(
            txnRef, plan.PriceVnd, $"Thanh toan goi {plan.Name} SameMess", clientIp);

        return new CreateOrderResultDto { TxnRef = txnRef, AmountVnd = plan.PriceVnd, PaymentUrl = url };
    }

    public async Task<PaymentCallbackResult> HandleReturnAsync(IDictionary<string, string> query)
    {
        var result = _gateway.ParseAndVerify(query);

        // IPN (server→server) là thẩm quyền chính, NHƯNG không gọi được localhost; nên kích hoạt
        // luôn ở Return khi chữ ký hợp lệ + thành công. Idempotent: IPN/Return cái nào tới trước thì kích hoạt.
        if (result.IsSuccess)
        {
            var order = await _orderRepository.GetByTxnRefAsync(result.TxnRef);
            if (order is not null
                && order.Status != PaymentStatus.Paid
                && order.AmountVnd == result.AmountVnd)
            {
                await MarkPaidAndActivateAsync(order, result.TransactionNo, result.ResponseCode);
            }
        }

        return result;
    }

    public async Task<(string RspCode, string Message)> HandleIpnAsync(IDictionary<string, string> query)
    {
        var result = _gateway.ParseAndVerify(query);
        if (!result.SignatureValid)
            return ("97", "Invalid signature");

        var order = await _orderRepository.GetByTxnRefAsync(result.TxnRef);
        if (order is null)
            return ("01", "Order not found");

        if (order.AmountVnd != result.AmountVnd)
            return ("04", "Invalid amount");

        if (order.Status == PaymentStatus.Paid)
            return ("02", "Order already confirmed");

        if (result.ResponseCode == "00")
        {
            await MarkPaidAndActivateAsync(order, result.TransactionNo, result.ResponseCode);
        }
        else
        {
            order.Status = PaymentStatus.Failed;
            order.VnpResponseCode = result.ResponseCode;
            await _orderRepository.SaveChangesAsync();
        }

        return ("00", "Confirm Success");
    }

    public async Task<SubscriptionDto> MockConfirmAsync(Guid userId, string txnRef)
    {
        var order = await _orderRepository.GetByTxnRefAsync(txnRef)
            ?? throw new NotFoundException("Order", txnRef);
        if (order.UserId != userId)
            throw new ForbiddenException("Đơn này không thuộc về bạn.");
        if (order.Status != PaymentStatus.Paid)
            await MarkPaidAndActivateAsync(order, "MOCK", "00");

        return await GetMySubscriptionAsync(userId);
    }

    public async Task<PayOsCreateResultDto> CreatePayOsOrderAsync(Guid userId, string planCode)
    {
        if (!PlanCode.IsValidPaid(planCode))
            throw new BadRequestException("Chỉ mua được gói Plus hoặc Gold.");

        var plan = await _planRepository.GetByCodeAsync(planCode);
        if (plan is null || !plan.IsActive)
            throw new NotFoundException("Plan", planCode);

        // orderCode: số nguyên dương duy nhất (giây epoch * 1000 + random) — PayOS yêu cầu dạng number
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + Random.Shared.Next(0, 1000);

        var order = new PaymentOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TxnRef = orderCode.ToString(CultureInfo.InvariantCulture),
            PlanCode = plan.Code,
            AmountVnd = plan.PriceVnd,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        // description ≤ 25 ký tự
        var result = await _payos.CreatePaymentAsync(orderCode, plan.PriceVnd, $"SameMess goi {plan.Code}");
        return result;
    }

    public async Task<bool> TryHandlePayOsWebhookAsync(PayOsWebhookResult v)
    {
        var order = await _orderRepository.GetByTxnRefAsync(v.OrderCode.ToString(CultureInfo.InvariantCulture));
        if (order is null) return false; // không phải đơn mua gói

        if (v.Success
            && order.Status != PaymentStatus.Paid
            && order.AmountVnd == v.AmountVnd)
        {
            await MarkPaidAndActivateAsync(order, $"PAYOS:{v.OrderCode}", "00");
        }
        return true;
    }

    /// <summary>Đánh dấu đơn đã trả + tạo/gia hạn thuê bao (gia hạn nối từ mốc còn lại nếu chưa hết hạn).</summary>
    private async Task MarkPaidAndActivateAsync(PaymentOrder order, string? transactionNo, string responseCode)
    {
        var now = DateTime.UtcNow;
        order.Status = PaymentStatus.Paid;
        order.PaidAt = now;
        order.VnpTransactionNo = transactionNo;
        order.VnpResponseCode = responseCode;

        var plan = await _planRepository.GetByCodeAsync(order.PlanCode);
        var days = plan?.DurationDays ?? 30;

        var sub = await _subscriptionRepository.GetByUserAsync(order.UserId);
        if (sub is null)
        {
            await _subscriptionRepository.AddAsync(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = order.UserId,
                PlanCode = order.PlanCode,
                StartAt = now,
                ExpiresAt = now.AddDays(days),
                UpdatedAt = now,
            });
        }
        else
        {
            var basis = sub.ExpiresAt > now ? sub.ExpiresAt : now; // còn hạn thì nối tiếp
            sub.PlanCode = order.PlanCode;
            sub.ExpiresAt = basis.AddDays(days);
            sub.UpdatedAt = now;
        }

        await _subscriptionRepository.SaveChangesAsync();
    }

    private static EntitlementsDto ToDto(PlanEntitlements e) => new()
    {
        UnlimitedLikes = e.UnlimitedLikes,
        DailyLikeLimit = e.DailyLikeLimit,
        CanUndo = e.CanUndo,
        CanBoost = e.CanBoost,
        CanSeeLikedMePhotos = e.CanSeeLikedMePhotos,
        SuperLikesPerDay = e.SuperLikesPerDay,
    };
}
