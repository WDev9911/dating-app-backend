using SameMess.Application.DTOs.DatePass;

namespace SameMess.Application.Interfaces.Services;

public interface IDatePassService
{
    Task<List<VenueComboDto>> GetCombosAsync();
    Task<List<EligibleMatchDto>> GetEligibleMatchesAsync(Guid userId);
    Task<DatePassOrderDto> CreateOrderAsync(Guid userId, CreateDatePassOrderDto dto);
    Task<DatePassOrderDto> ConfirmAsync(Guid userId, Guid orderId);
    Task<DatePassOrderDto> RedeemAsync(Guid userId, Guid orderId);
    Task<List<DatePassOrderDto>> GetMyOrdersAsync(Guid userId);
    Task<DatePassRevenueDto> GetRevenueAsync();

    // Admin
    Task<List<VenueComboDto>> AdminListCombosAsync();
    Task<VenueComboDto> AdminCreateComboAsync(ComboPayloadDto dto);
    Task AdminDeleteComboAsync(Guid id);
}
