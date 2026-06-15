using SameMess.Application.DTOs.Settings;

namespace SameMess.Application.Interfaces.Services;

public interface IInterestsService
{
    Task<List<InterestDto>> GetCatalogAsync();
    Task<List<InterestDto>> GetMyInterestsAsync(Guid userId);
    Task<List<InterestDto>> UpdateMyInterestsAsync(Guid userId, UpdateInterestsDto dto);
}
