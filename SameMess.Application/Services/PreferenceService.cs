using AutoMapper;
using SameMess.Application.DTOs.Preference;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class PreferenceService : IPreferenceService
{
    private readonly IUserPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;

    public PreferenceService(IUserPreferenceRepository preferenceRepository, IMapper mapper)
    {
        _preferenceRepository = preferenceRepository;
        _mapper = mapper;
    }

    public async Task<PreferenceDto> GetMyPreferenceAsync(Guid userId)
    {
        var preference = await _preferenceRepository.GetByUserIdAsync(userId)
            ?? await CreateDefaultAsync(userId);

        return _mapper.Map<PreferenceDto>(preference);
    }

    public async Task<PreferenceDto> UpdatePreferenceAsync(Guid userId, UpdatePreferenceDto dto)
    {
        var preference = await _preferenceRepository.GetByUserIdAsync(userId);

        if (preference == null)
        {
            preference = new UserPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };
            await _preferenceRepository.AddAsync(preference);
        }

        preference.InterestedInGender = dto.InterestedInGender;
        preference.MinAge = dto.MinAge;
        preference.MaxAge = dto.MaxAge;
        preference.MaxDistanceKm = dto.MaxDistanceKm;
        preference.UpdatedAt = DateTime.UtcNow;

        await _preferenceRepository.SaveChangesAsync();
        return _mapper.Map<PreferenceDto>(preference);
    }

    private async Task<UserPreference> CreateDefaultAsync(Guid userId)
    {
        var preference = new UserPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            // các giá trị mặc định (Everyone / 18 / 99 / 50km) lấy từ entity
        };
        await _preferenceRepository.AddAsync(preference);
        await _preferenceRepository.SaveChangesAsync();
        return preference;
    }
}
