using SameMess.Application.Common;
using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Connection;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class VenueService : IVenueService
{
    public const int UnlockLevel = 4;          // cây phải đạt Level này mới mở khóa hẹn hò
    private const int DefaultRadiusKm = 10;
    private const int MaxRadiusKm = 30;
    private const int MaxResults = 30;

    private readonly IVenueRepository _venueRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchPlantRepository _plantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public VenueService(
        IVenueRepository venueRepository,
        IMatchRepository matchRepository,
        IMatchPlantRepository plantRepository,
        IUserRepository userRepository,
        IAuditService auditService)
    {
        _venueRepository = venueRepository;
        _matchRepository = matchRepository;
        _plantRepository = plantRepository;
        _userRepository = userRepository;
        _auditService = auditService;
    }

    public async Task<List<VenueDto>> GetNearbyForMatchAsync(Guid userId, Guid matchId, string? category, int? radiusKm)
    {
        var match = await _matchRepository.GetByIdForUserAsync(matchId, userId)
            ?? throw new NotFoundException("Match", matchId);

        // Gate: cây của cặp phải đạt Level >= 4
        var plant = await _plantRepository.GetByMatchIdAsync(matchId);
        if (plant is null || plant.Level < UnlockLevel)
            throw new ForbiddenException($"Chăm cây tình yêu đạt Level {UnlockLevel} để mở khóa gợi ý hẹn hò.");

        var partnerId = match.UserAId == userId ? match.UserBId : match.UserAId;
        var users = await _userRepository.GetWithProfileByIdsAsync(new[] { userId, partnerId });
        var me = users.FirstOrDefault(u => u.Id == userId)?.Profile;
        var partner = users.FirstOrDefault(u => u.Id == partnerId)?.Profile;

        if (me?.Latitude is null || me.Longitude is null || partner?.Latitude is null || partner.Longitude is null)
            throw new BadRequestException("Cả hai cần có vị trí để gợi ý địa điểm.");

        // Điểm giữa 2 người (công bằng cho cả hai)
        var midLat = (me.Latitude.Value + partner.Latitude.Value) / 2;
        var midLon = (me.Longitude.Value + partner.Longitude.Value) / 2;
        var radius = Math.Clamp(radiusKm ?? DefaultRadiusKm, 1, MaxRadiusKm);

        var venues = await _venueRepository.GetActiveAsync(category);

        return venues
            .Select(v => new { v, dist = GeoCalculator.DistanceKm(midLat, midLon, v.Latitude, v.Longitude) })
            .Where(x => x.dist <= radius)
            .OrderBy(x => x.dist)
            .Take(MaxResults)
            .Select(x => ToDto(x.v, x.dist))
            .ToList();
    }

    public async Task<List<VenueDto>> ListAsync(bool includeInactive)
    {
        var venues = await _venueRepository.GetAllAsync(includeInactive);
        return venues.Select(v => ToDto(v, null)).ToList();
    }

    public async Task<VenueDto> CreateAsync(Guid adminId, VenuePayloadDto dto)
    {
        var v = new Venue
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Category = dto.Category,
            Address = dto.Address,
            District = dto.District,
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ImageUrl = dto.ImageUrl,
            PriceRange = dto.PriceRange,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
        };
        await _venueRepository.AddAsync(v);
        await _venueRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "venue.create", "Venue", v.Id, v.Name);
        return ToDto(v, null);
    }

    public async Task<VenueDto> UpdateAsync(Guid adminId, Guid id, VenuePayloadDto dto)
    {
        var v = await _venueRepository.GetByIdAsync(id) ?? throw new NotFoundException("Venue", id);
        v.Name = dto.Name; v.Category = dto.Category; v.Address = dto.Address;
        v.District = dto.District; v.City = dto.City;
        v.Latitude = dto.Latitude; v.Longitude = dto.Longitude;
        v.ImageUrl = dto.ImageUrl; v.PriceRange = dto.PriceRange;
        v.Description = dto.Description; v.IsActive = dto.IsActive;
        await _venueRepository.UpdateAsync(v);
        await _venueRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "venue.update", "Venue", id);
        return ToDto(v, null);
    }

    public async Task DeleteAsync(Guid adminId, Guid id)
    {
        var v = await _venueRepository.GetByIdAsync(id) ?? throw new NotFoundException("Venue", id);
        await _venueRepository.DeleteAsync(v);
        await _venueRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "venue.delete", "Venue", id, v.Name);
    }

    private static VenueDto ToDto(Venue v, double? distanceKm) => new()
    {
        Id = v.Id, Name = v.Name, Category = v.Category, Address = v.Address,
        District = v.District, City = v.City, Latitude = v.Latitude, Longitude = v.Longitude,
        ImageUrl = v.ImageUrl, PriceRange = v.PriceRange, Description = v.Description,
        DistanceKm = distanceKm.HasValue ? (int)Math.Max(0, Math.Round(distanceKm.Value)) : 0,
    };
}
