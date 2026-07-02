using System.Globalization;
using SameMess.Application.DTOs.Billing;
using SameMess.Application.DTOs.DatePass;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class DatePassService : IDatePassService
{
    private const int UnlockLevel = 4;       // cây phải đạt Cấp 4 mới được đặt combo
    private const int VoucherValidDays = 14;

    private readonly IVenueComboRepository _comboRepository;
    private readonly IDatePassOrderRepository _orderRepository;
    private readonly IMatchService _matchService;
    private readonly IMatchPlantRepository _plantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IPayOsGateway _payos;

    public DatePassService(
        IVenueComboRepository comboRepository,
        IDatePassOrderRepository orderRepository,
        IMatchService matchService,
        IMatchPlantRepository plantRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        INotificationService notificationService,
        IPayOsGateway payos)
    {
        _comboRepository = comboRepository;
        _orderRepository = orderRepository;
        _matchService = matchService;
        _plantRepository = plantRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _notificationService = notificationService;
        _payos = payos;
    }

    public async Task<List<VenueComboDto>> GetCombosAsync()
    {
        var combos = await _comboRepository.GetActiveWithVenueAsync();
        return combos.Select(ToComboDto).ToList();
    }

    public async Task<List<EligibleMatchDto>> GetEligibleMatchesAsync(Guid userId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        var result = new List<EligibleMatchDto>();
        foreach (var m in matches)
        {
            var plant = await _plantRepository.GetByMatchIdAsync(m.MatchId);
            var level = plant?.Level ?? 1;
            if (level < UnlockLevel) continue;
            result.Add(new EligibleMatchDto
            {
                MatchId = m.MatchId,
                DisplayName = m.DisplayName,
                AvatarUrl = m.AvatarUrl,
                Level = level,
            });
        }
        return result;
    }

    public async Task<DatePassOrderDto> CreateOrderAsync(Guid userId, CreateDatePassOrderDto dto)
    {
        var order = await BuildPendingOrderAsync(userId, dto, null);
        return ToOrderDto(order, userId, order.PartnerName ?? "Người ấy");
    }

    /// <summary>Tạo đơn ưu đãi + link thanh toán PayOS thật (VietQR).</summary>
    public async Task<PayOsCreateResultDto> CreatePayOsOrderAsync(Guid userId, CreateDatePassOrderDto dto)
    {
        // orderCode duy nhất (giống luồng Premium) — lưu vào đơn để đối chiếu webhook.
        var orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 + Random.Shared.Next(0, 1000);
        var order = await BuildPendingOrderAsync(userId, dto, orderCode);

        var baseUrl = _payos.FrontendBaseUrl;
        return await _payos.CreatePaymentAsync(
            orderCode,
            order.AmountVnd,
            "SameMess uu dai",
            returnUrl: $"{baseUrl}/date-pass?payment=success",
            cancelUrl: $"{baseUrl}/date-pass?payment=cancel");
    }

    /// <summary>[DEV/mock] Đánh dấu đã trả tiền → phát voucher + gửi email.</summary>
    public async Task<DatePassOrderDto> ConfirmAsync(Guid userId, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Đơn", orderId);
        if (order.BuyerId != userId)
            throw new ForbiddenException("Bạn không phải người đặt đơn này.");
        if (order.Status != DatePassStatus.Pending)
            throw new BadRequestException("Đơn đã được xử lý.");

        await PayAndDispatchAsync(order);
        return ToOrderDto(order, userId, order.PartnerName ?? "Người ấy");
    }

    /// <summary>Webhook PayOS đã verify → nếu là đơn ưu đãi thì đánh dấu Paid + gửi voucher.</summary>
    public async Task<bool> TryHandlePayOsWebhookAsync(PayOsWebhookResult webhook)
    {
        var order = await _orderRepository.GetByPayOsOrderCodeAsync(webhook.OrderCode);
        if (order is null) return false; // không phải đơn ưu đãi

        if (webhook.Success
            && order.Status == DatePassStatus.Pending
            && order.AmountVnd == webhook.AmountVnd)
        {
            await PayAndDispatchAsync(order);
        }
        return true;
    }

    public async Task<bool> VerifyPayOsPaymentAsync(long orderCode)
    {
        var order = await _orderRepository.GetByPayOsOrderCodeAsync(orderCode);
        if (order is null) return false; // không phải đơn ưu đãi

        if (order.Status == DatePassStatus.Pending)
        {
            var status = await _payos.GetPaymentStatusAsync(orderCode);
            if (status.Paid && (status.AmountVnd == 0 || status.AmountVnd == order.AmountVnd))
                await PayAndDispatchAsync(order);
        }
        return true;
    }

    /// <summary>Quán quét QR (trong app) xác nhận đã sử dụng — dành cho user đã đăng nhập.</summary>
    public async Task<DatePassOrderDto> RedeemAsync(Guid userId, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Đơn", orderId);
        MarkRedeemed(order);
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        var name = await PartnerNameAsync(userId, order.MatchId);
        return ToOrderDto(order, userId, name);
    }

    // ── Trang voucher công khai (quán quét QR mở ra, không cần đăng nhập) ──
    public async Task<VoucherPublicDto> GetVoucherAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Voucher", orderId);
        return ToVoucherPublicDto(order);
    }

    public async Task<VoucherPublicDto> RedeemVoucherPublicAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Voucher", orderId);
        MarkRedeemed(order);
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();
        return ToVoucherPublicDto(order);
    }

    public async Task<List<DatePassOrderDto>> GetMyOrdersAsync(Guid userId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        var nameByMatch = matches.ToDictionary(m => m.MatchId, m => m.DisplayName);
        var orders = await _orderRepository.GetForMatchesAsync(nameByMatch.Keys);
        return orders.Select(o => ToOrderDto(o, userId, nameByMatch.GetValueOrDefault(o.MatchId, o.PartnerName ?? "Người ấy"))).ToList();
    }

    public async Task<DatePassRevenueDto> GetRevenueAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();
        var paid = orders.Where(o => o.Status == DatePassStatus.Paid || o.Status == DatePassStatus.Redeemed).ToList();
        return new DatePassRevenueDto
        {
            TotalOrders = orders.Count,
            PaidOrders = paid.Count,
            RedeemedOrders = orders.Count(o => o.Status == DatePassStatus.Redeemed),
            GmvVnd = paid.Sum(o => (long)o.AmountVnd),
            CommissionVnd = paid.Sum(o => (long)o.CommissionVnd),
        };
    }

    // ── Admin ──
    public async Task<List<VenueComboDto>> AdminListCombosAsync()
    {
        var combos = await _comboRepository.GetAllWithVenueAsync();
        return combos.Select(ToComboDto).ToList();
    }

    public async Task<VenueComboDto> AdminCreateComboAsync(ComboPayloadDto dto)
    {
        var combo = new VenueCombo
        {
            Id = Guid.NewGuid(),
            VenueId = dto.VenueId,
            Title = dto.Title,
            Description = dto.Description,
            OriginalPriceVnd = dto.OriginalPriceVnd,
            SalePriceVnd = dto.SalePriceVnd,
            CommissionPercent = dto.CommissionPercent,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
        };
        await _comboRepository.AddAsync(combo);
        await _comboRepository.SaveChangesAsync();
        var saved = await _comboRepository.GetWithVenueAsync(combo.Id);
        return ToComboDto(saved!);
    }

    public async Task AdminDeleteComboAsync(Guid id)
    {
        var combo = await _comboRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Combo", id);
        await _comboRepository.DeleteAsync(combo);
        await _comboRepository.SaveChangesAsync();
    }

    // ── helpers ──

    /// <summary>
    /// Tạo đơn Pending (dùng chung cho luồng mock lẫn PayOS): validate combo/match/cây,
    /// chặn mua trùng, snapshot tên + email cả hai người, sinh mã voucher.
    /// </summary>
    private async Task<DatePassOrder> BuildPendingOrderAsync(Guid userId, CreateDatePassOrderDto dto, long? payOsOrderCode)
    {
        var combo = await _comboRepository.GetWithVenueAsync(dto.ComboId)
            ?? throw new NotFoundException("Combo", dto.ComboId);
        if (!combo.IsActive) throw new BadRequestException("Combo không còn khả dụng.");

        var matches = await _matchService.GetMyMatchesAsync(userId);
        var match = matches.FirstOrDefault(m => m.MatchId == dto.MatchId)
            ?? throw new ForbiddenException("Bạn không thuộc cặp match này.");
        var plant = await _plantRepository.GetByMatchIdAsync(dto.MatchId);
        if ((plant?.Level ?? 1) < UnlockLevel)
            throw new ForbiddenException($"Cần chăm cây đạt Cấp {UnlockLevel} để đặt combo hẹn hò.");

        // Chống mua trùng: cặp đã có voucher đang hiệu lực cho đúng loại combo này
        var existing = await _orderRepository.GetActiveForCoupleComboAsync(dto.MatchId, dto.ComboId);
        if (existing is not null)
            throw new ConflictException("Cặp của bạn đã có voucher cho combo này. Hãy dùng hoặc đợi hết hạn trước khi mua lại.");

        // CHỐNG LẠM DỤNG: voucher luôn gửi tới EMAIL ĐĂNG KÝ của CẢ HAI người (không cho nhập tay).
        var buyer = await _userRepository.GetByIdAsync(userId);
        var partner = await _userRepository.GetByIdAsync(match.UserId);

        var now = DateTime.UtcNow;
        var order = new DatePassOrder
        {
            Id = Guid.NewGuid(),
            MatchId = dto.MatchId,
            BuyerId = userId,
            PartnerId = match.UserId,
            VenueId = combo.VenueId,
            ComboId = combo.Id,
            VenueName = combo.Venue.Name,
            ComboTitle = combo.Title,
            BuyerName = buyer?.Profile?.DisplayName ?? "Người mua",
            PartnerName = match.DisplayName,
            AmountVnd = combo.SalePriceVnd,
            CommissionVnd = combo.SalePriceVnd * combo.CommissionPercent / 100,
            VoucherCode = GenerateVoucherCode(),
            PayOsOrderCode = payOsOrderCode,
            Email = buyer?.Email,
            PartnerEmail = partner?.Email,
            Status = DatePassStatus.Pending,
            CreatedAt = now,
            ExpiresAt = now.AddDays(VoucherValidDays),
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();
        return order;
    }

    /// <summary>Đánh dấu Paid → gửi voucher (QR trỏ tới trang voucher) cho cả hai + báo đối phương. Fail-safe.</summary>
    private async Task PayAndDispatchAsync(DatePassOrder order)
    {
        order.Status = DatePassStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        var model = new VoucherEmailModel
        {
            VenueName = order.VenueName,
            ComboTitle = order.ComboTitle,
            AmountVnd = order.AmountVnd,
            VoucherCode = order.VoucherCode,
            QrUrl = VoucherQrUrl(order.Id),
            ExpiresAt = order.ExpiresAt,
        };
        var recipients = new[] { order.Email, order.PartnerEmail }
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase);
        foreach (var to in recipients)
        {
            try { await _emailService.SendVoucherEmailAsync(to, model); }
            catch { /* bỏ qua lỗi gửi email — voucher vẫn hiện trong app */ }
        }

        // Báo cho đối phương biết (minh bạch + chống lạm dụng âm thầm) — fail-safe
        if (order.PartnerId is Guid partnerId)
        {
            try
            {
                await _notificationService.NotifyAsync(partnerId, NotificationType.Match,
                    "Combo hẹn hò mới 🎟️",
                    $"Người ấy vừa đặt \"{order.ComboTitle}\" tại {order.VenueName} cho buổi hẹn của hai bạn. Kiểm tra email để nhận voucher!",
                    order.Id.ToString());
            }
            catch { /* không để thông báo làm hỏng luồng */ }
        }
    }

    private static void MarkRedeemed(DatePassOrder order)
    {
        if (order.Status == DatePassStatus.Redeemed)
            throw new BadRequestException("Voucher đã được sử dụng.");
        if (order.Status != DatePassStatus.Paid)
            throw new BadRequestException("Voucher chưa thanh toán nên không thể sử dụng.");
        if (order.ExpiresAt < DateTime.UtcNow)
            throw new BadRequestException("Voucher đã hết hạn.");

        order.Status = DatePassStatus.Redeemed;
        order.RedeemedAt = DateTime.UtcNow;
    }

    private async Task<string> PartnerNameAsync(Guid userId, Guid matchId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        return matches.FirstOrDefault(m => m.MatchId == matchId)?.DisplayName ?? "Người ấy";
    }

    /// <summary>QR chứa LINK trang voucher công khai → quét ra hiện đầy đủ thông tin.</summary>
    private string VoucherQrUrl(Guid orderId)
    {
        var target = $"{_payos.FrontendBaseUrl}/voucher/{orderId}";
        return $"https://api.qrserver.com/v1/create-qr-code/?size=240x240&data={Uri.EscapeDataString(target)}";
    }

    private static string GenerateVoucherCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var rnd = Random.Shared;
        var s = new string(Enumerable.Range(0, 8).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        return $"SM-{s[..4]}-{s[4..]}";
    }

    private static VenueComboDto ToComboDto(VenueCombo c) => new()
    {
        Id = c.Id,
        VenueId = c.VenueId,
        VenueName = c.Venue.Name,
        Category = c.Venue.Category,
        VenueAddress = c.Venue.Address,
        VenueImageUrl = c.Venue.ImageUrl,
        Title = c.Title,
        Description = c.Description,
        OriginalPriceVnd = c.OriginalPriceVnd,
        SalePriceVnd = c.SalePriceVnd,
        DiscountPercent = c.OriginalPriceVnd > 0
            ? (int)Math.Round((1 - (double)c.SalePriceVnd / c.OriginalPriceVnd) * 100)
            : 0,
        CommissionPercent = c.CommissionPercent,
    };

    private DatePassOrderDto ToOrderDto(DatePassOrder o, Guid userId, string partnerName) => new()
    {
        Id = o.Id,
        MatchId = o.MatchId,
        PartnerName = partnerName,
        VenueName = o.VenueName,
        ComboTitle = o.ComboTitle,
        AmountVnd = o.AmountVnd,
        CommissionVnd = o.CommissionVnd,
        VoucherCode = o.VoucherCode,
        QrUrl = VoucherQrUrl(o.Id),
        Status = o.Status,
        Email = o.Email,
        IsMine = o.BuyerId == userId,
        CreatedAt = o.CreatedAt,
        ExpiresAt = o.ExpiresAt,
        RedeemedAt = o.RedeemedAt,
    };

    private static VoucherPublicDto ToVoucherPublicDto(DatePassOrder o)
    {
        var expired = o.ExpiresAt < DateTime.UtcNow;
        return new VoucherPublicDto
        {
            Id = o.Id,
            BuyerName = o.BuyerName ?? "Người mua",
            PartnerName = o.PartnerName ?? "Người ấy",
            VenueName = o.VenueName,
            ComboTitle = o.ComboTitle,
            AmountVnd = o.AmountVnd,
            VoucherCode = o.VoucherCode,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            ExpiresAt = o.ExpiresAt,
            RedeemedAt = o.RedeemedAt,
            IsExpired = expired,
            CanRedeem = o.Status == DatePassStatus.Paid && !expired,
        };
    }
}
