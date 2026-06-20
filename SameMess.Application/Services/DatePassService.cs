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

    public DatePassService(
        IVenueComboRepository comboRepository,
        IDatePassOrderRepository orderRepository,
        IMatchService matchService,
        IMatchPlantRepository plantRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        INotificationService notificationService)
    {
        _comboRepository = comboRepository;
        _orderRepository = orderRepository;
        _matchService = matchService;
        _plantRepository = plantRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _notificationService = notificationService;
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
        // Combo hợp lệ
        var combo = await _comboRepository.GetWithVenueAsync(dto.ComboId)
            ?? throw new NotFoundException("Combo", dto.ComboId);
        if (!combo.IsActive) throw new BadRequestException("Combo không còn khả dụng.");

        // Match phải thuộc về user + đạt Level 4
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

        // CHỐNG LẠM DỤNG: voucher luôn gửi tới EMAIL ĐĂNG KÝ của CẢ HAI người trong cặp
        // (không cho nhập tay) → chắc chắn đến đúng 2 người match.
        var email = (await _userRepository.GetByIdAsync(userId))?.Email;
        var partnerEmail = (await _userRepository.GetByIdAsync(match.UserId))?.Email;

        var now = DateTime.UtcNow;
        var order = new DatePassOrder
        {
            Id = Guid.NewGuid(),
            MatchId = dto.MatchId,
            BuyerId = userId,
            VenueId = combo.VenueId,
            ComboId = combo.Id,
            VenueName = combo.Venue.Name,
            ComboTitle = combo.Title,
            AmountVnd = combo.SalePriceVnd,
            CommissionVnd = combo.SalePriceVnd * combo.CommissionPercent / 100,
            VoucherCode = GenerateVoucherCode(),
            Email = email,
            PartnerEmail = partnerEmail,
            Status = DatePassStatus.Pending,
            CreatedAt = now,
            ExpiresAt = now.AddDays(VoucherValidDays),
        };
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();

        return ToOrderDto(order, userId, match.DisplayName);
    }

    public async Task<DatePassOrderDto> ConfirmAsync(Guid userId, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Đơn", orderId);
        if (order.BuyerId != userId)
            throw new ForbiddenException("Bạn không phải người đặt đơn này.");
        if (order.Status != DatePassStatus.Pending)
            throw new BadRequestException("Đơn đã được xử lý.");

        order.Status = DatePassStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        // Gửi email voucher tới CẢ HAI (cùng 1 mã) — fail-safe: lỗi email không làm hỏng thanh toán
        var model = new VoucherEmailModel
        {
            VenueName = order.VenueName,
            ComboTitle = order.ComboTitle,
            AmountVnd = order.AmountVnd,
            VoucherCode = order.VoucherCode,
            QrUrl = QrUrl(order.VoucherCode),
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
        var matches = await _matchService.GetMyMatchesAsync(userId);
        var partner = matches.FirstOrDefault(m => m.MatchId == order.MatchId);
        if (partner is not null)
        {
            try
            {
                await _notificationService.NotifyAsync(partner.UserId, NotificationType.Match,
                    "Combo hẹn hò mới 🎟️",
                    $"Người ấy vừa đặt \"{order.ComboTitle}\" tại {order.VenueName} cho buổi hẹn của hai bạn. Kiểm tra email để nhận voucher!",
                    order.Id.ToString());
            }
            catch { /* không để thông báo làm hỏng luồng */ }
        }

        return ToOrderDto(order, userId, partner?.DisplayName ?? "Người ấy");
    }

    public async Task<DatePassOrderDto> RedeemAsync(Guid userId, Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId)
            ?? throw new NotFoundException("Đơn", orderId);
        if (order.Status == DatePassStatus.Redeemed)
            throw new BadRequestException("Voucher đã được sử dụng.");
        if (order.Status != DatePassStatus.Paid)
            throw new BadRequestException("Voucher chưa thanh toán nên không thể sử dụng.");

        order.Status = DatePassStatus.Redeemed;
        order.RedeemedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();

        var name = await PartnerNameAsync(userId, order.MatchId);
        return ToOrderDto(order, userId, name);
    }

    public async Task<List<DatePassOrderDto>> GetMyOrdersAsync(Guid userId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        var nameByMatch = matches.ToDictionary(m => m.MatchId, m => m.DisplayName);
        var orders = await _orderRepository.GetForMatchesAsync(nameByMatch.Keys);
        return orders.Select(o => ToOrderDto(o, userId, nameByMatch.GetValueOrDefault(o.MatchId, "Người ấy"))).ToList();
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
    private async Task<string> PartnerNameAsync(Guid userId, Guid matchId)
    {
        var matches = await _matchService.GetMyMatchesAsync(userId);
        return matches.FirstOrDefault(m => m.MatchId == matchId)?.DisplayName ?? "Người ấy";
    }

    private static string QrUrl(string code) =>
        $"https://api.qrserver.com/v1/create-qr-code/?size=240x240&data={Uri.EscapeDataString(code)}";

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

    private static DatePassOrderDto ToOrderDto(DatePassOrder o, Guid userId, string partnerName) => new()
    {
        Id = o.Id,
        MatchId = o.MatchId,
        PartnerName = partnerName,
        VenueName = o.VenueName,
        ComboTitle = o.ComboTitle,
        AmountVnd = o.AmountVnd,
        CommissionVnd = o.CommissionVnd,
        VoucherCode = o.VoucherCode,
        QrUrl = QrUrl(o.VoucherCode),
        Status = o.Status,
        Email = o.Email,
        IsMine = o.BuyerId == userId,
        CreatedAt = o.CreatedAt,
        ExpiresAt = o.ExpiresAt,
        RedeemedAt = o.RedeemedAt,
    };
}
