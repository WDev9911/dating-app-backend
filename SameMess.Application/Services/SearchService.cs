using AutoMapper;
using SameMess.Application.Common;
using SameMess.Application.DTOs.Discovery;
using SameMess.Application.DTOs.Profile;
using SameMess.Application.DTOs.Search;
using SameMess.Application.DTOs.Settings;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class SearchService : ISearchService
{
    private const int FetchCap = 500;
    private const int MaxResults = 100;

    private readonly ISearchRepository _searchRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly IMapper _mapper;

    public SearchService(
        ISearchRepository searchRepository,
        IInterestRepository interestRepository,
        IUserRepository userRepository,
        IBlockRepository blockRepository,
        IMapper mapper)
    {
        _searchRepository = searchRepository;
        _interestRepository = interestRepository;
        _userRepository = userRepository;
        _blockRepository = blockRepository;
        _mapper = mapper;
    }

    public async Task<SearchFiltersDto> GetFiltersAsync(Guid userId)
    {
        var interests = await _interestRepository.GetActiveAsync();
        var cities = await _searchRepository.GetDistinctCitiesAsync(200);

        return new SearchFiltersDto
        {
            Interests = interests
                .Select(i => new InterestDto { Id = i.Id, Name = i.Name, GroupName = i.GroupName })
                .ToList(),
            Cities = cities,
            Genders = Gender.All.ToList(),
            Personalities = new List<string>(), // chưa hỗ trợ lọc theo tính cách ở backend
        };
    }

    public async Task<List<DiscoveryProfileDto>> SearchAsync(Guid userId, SearchQueryDto q)
    {
        var me = await _userRepository.GetFullProfileAsync(userId);
        var myProfile = me?.Profile;
        var hasMyLocation = myProfile?.Latitude != null && myProfile?.Longitude != null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly? minBirth = q.MaxAge.HasValue ? today.AddYears(-q.MaxAge.Value - 1) : null;
        DateOnly? maxBirth = q.MinAge.HasValue ? today.AddYears(-q.MinAge.Value) : null;

        var blocked = await _blockRepository.GetRelatedUserIdsAsync(userId);
        var interestIds = q.Interests?.Distinct().ToList() ?? new List<Guid>();

        var candidates = await _searchRepository.SearchAsync(
            userId, blocked, q.Gender, q.City, minBirth, maxBirth, interestIds, FetchCap);

        var rows = new List<(DiscoveryProfileDto Dto, int Age, double Distance, DateTime Created)>();
        foreach (var c in candidates)
        {
            var age = AgeCalculator.FromDateOfBirth(c.DateOfBirth) ?? 0;
            if (q.MinAge.HasValue && age < q.MinAge.Value) continue;
            if (q.MaxAge.HasValue && age > q.MaxAge.Value) continue;

            double distance = 0;
            if (hasMyLocation && c.Latitude != null && c.Longitude != null)
            {
                distance = GeoCalculator.DistanceKm(
                    myProfile!.Latitude!.Value, myProfile.Longitude!.Value, c.Latitude.Value, c.Longitude.Value);
                if (q.DistanceKm.HasValue && distance > q.DistanceKm.Value) continue;
            }

            rows.Add((MapToDto(c, age, distance), age, distance, c.CreatedAt));
        }

        IEnumerable<(DiscoveryProfileDto Dto, int Age, double Distance, DateTime Created)> ordered = (q.Sort?.ToLowerInvariant()) switch
        {
            "newest" => rows.OrderByDescending(r => r.Created),
            "age" => rows.OrderBy(r => r.Age),
            "distance" => rows.OrderBy(r => r.Distance),
            _ => hasMyLocation ? rows.OrderBy(r => r.Distance) : rows.OrderByDescending(r => r.Created),
        };

        return ordered.Take(MaxResults).Select(r => r.Dto).ToList();
    }

    private DiscoveryProfileDto MapToDto(UserProfile p, int age, double distanceKm)
    {
        var photos = p.User?.Photos ?? new List<Photo>();
        return new DiscoveryProfileDto
        {
            UserId = p.UserId,
            DisplayName = p.DisplayName,
            Age = age,
            Gender = p.Gender,
            Bio = p.Bio,
            Height = p.Height,
            Location = p.Location,
            DatingGoal = p.DatingGoal,
            DistanceKm = (int)Math.Max(0, Math.Round(distanceKm)),
            IsBoosted = p.BoostedUntil.HasValue && p.BoostedUntil.Value > DateTime.UtcNow,
            IsPhotoVerified = p.IsPhotoVerified,
            Photos = _mapper.Map<List<PhotoDto>>(photos.OrderBy(ph => ph.OrderIndex)),
        };
    }
}
