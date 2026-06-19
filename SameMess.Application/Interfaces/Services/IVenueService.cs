using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Connection;

namespace SameMess.Application.Interfaces.Services;

public interface IVenueService
{
    /// <summary>Gợi ý quán gần điểm giữa 2 người trong match — yêu cầu cây ≥ Level 4.</summary>
    Task<List<VenueDto>> GetNearbyForMatchAsync(Guid userId, Guid matchId, string? category, int? radiusKm);

    // Admin
    Task<List<VenueDto>> ListAsync(bool includeInactive);
    Task<VenueDto> CreateAsync(Guid adminId, VenuePayloadDto dto);
    Task<VenueDto> UpdateAsync(Guid adminId, Guid id, VenuePayloadDto dto);
    Task DeleteAsync(Guid adminId, Guid id);
}
