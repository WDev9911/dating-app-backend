using AutoMapper;
using SameMess.Application.Common;
using SameMess.Application.DTOs.Discovery;
using SameMess.Application.DTOs.Profile;
using SameMess.Application.Interfaces.Services;
using SameMess.Application.Reputation;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class DiscoveryService : IDiscoveryService
{
    // Trần số bản ghi kéo từ DB sau lọc thô (bounding box). Lọc tinh + sắp xếp làm trong bộ nhớ.
    // Giai đoạn đầu (ít dữ liệu) là đủ; khi scale lớn nên chuyển sang geography + spatial index.
    private const int FetchCap = 500;

    private readonly IUserRepository _userRepository;
    private readonly IDiscoveryRepository _discoveryRepository;
    private readonly ISwipeRepository _swipeRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IReputationService _reputationService;
    private readonly IMapper _mapper;

    public DiscoveryService(
        IUserRepository userRepository,
        IDiscoveryRepository discoveryRepository,
        ISwipeRepository swipeRepository,
        IBlockRepository blockRepository,
        IMatchRepository matchRepository,
        IReputationService reputationService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _discoveryRepository = discoveryRepository;
        _swipeRepository = swipeRepository;
        _blockRepository = blockRepository;
        _matchRepository = matchRepository;
        _reputationService = reputationService;
        _mapper = mapper;
    }

    public async Task<List<DiscoveryProfileDto>> GetFeedAsync(Guid userId, int limit, bool includeSwiped = false)
    {
        var me = await _userRepository.GetFullProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var profile = me.Profile;
        var isAdminViewer = me.Role == UserRole.Admin;

        // Admin không bị bắt hoàn thiện hồ sơ như user thường (họ có thể chưa có info/location/ảnh)
        if (!isAdminViewer && (profile is null
            || !profile.IsProfileCompleted
            || profile.Latitude is null
            || profile.Longitude is null))
        {
            throw new ForbiddenException(
                "Please complete your profile (info, location and at least one photo) before using Discovery.");
        }
        if (profile is null)
            return new List<DiscoveryProfileDto>(); // admin chưa có hồ sơ nào — chưa có gì để tính khoảng cách/tuổi

        // Preferences: nếu chưa có thì dùng mặc định rộng
        var interestedIn = me.Preference?.InterestedInGender ?? GenderPreference.Everyone;
        var minAge = me.Preference?.MinAge ?? 18;
        var maxAge = me.Preference?.MaxAge ?? 99;
        var maxDistanceKm = me.Preference?.MaxDistanceKm ?? 50;

        // Admin có thể chưa set vị trí — khi đó bỏ qua lọc khoảng cách thay vì crash
        var hasMyLocation = profile.Latitude is not null && profile.Longitude is not null;
        var myLat = profile.Latitude ?? 0;
        var myLon = profile.Longitude ?? 0;

        // Khoảng tuổi -> khoảng ngày sinh (đệm thêm 1 năm để lọc thô; lọc tinh lại theo tuổi ở dưới)
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var maxBirthDate = today.AddYears(-minAge);       // sinh muộn nhất => trẻ nhất đúng minAge
        var minBirthDate = today.AddYears(-maxAge - 1);   // sinh sớm nhất (có đệm)

        var (minLat, maxLat, minLon, maxLon) = hasMyLocation
            ? GeoCalculator.BoundingBox(myLat, myLon, maxDistanceKm)
            : (-90.0, 90.0, -180.0, 180.0); // không có vị trí -> không lọc theo khoảng cách (toàn cầu)

        var requiredGender = interestedIn == GenderPreference.Everyone ? null : interestedIn;

        // Thông tin của tôi để lọc 2 chiều (tôi có nằm trong tiêu chí của họ không)
        var myGender = profile.Gender;
        var myAge = AgeCalculator.FromDateOfBirth(profile.DateOfBirth) ?? 0;

        // Loại khỏi feed: người tôi đã swipe + người có quan hệ block (2 chiều)
        // + người đã SuperLike mình (họ chỉ hiển thị ở mục riêng /superliked-me, tránh trùng)
        // includeSwiped = true (nút "Tải lại gợi ý"): hiện lại tất cả, chỉ loại block + superLiker.
        var alreadySwiped = includeSwiped
            ? new List<Guid>()
            : await _swipeRepository.GetSwipedTargetIdsAsync(userId);
        var blockedRelated = await _blockRepository.GetRelatedUserIdsAsync(userId);
        var superLikers = (await _swipeRepository.GetSuperLikersAsync(userId)).Select(s => s.SwiperId);
        // Luôn loại người đã match (kể cả khi includeSwiped) — không hiện lại người đã ghép đôi
        var matchedPartners = (await _matchRepository.GetActiveForUserAsync(userId))
            .Select(m => m.UserAId == userId ? m.UserBId : m.UserAId);
        var excludeIds = alreadySwiped.Concat(blockedRelated).Concat(superLikers)
            .Concat(matchedPartners).Distinct().ToList();

        var candidates = await _discoveryRepository.GetCandidatesAsync(
            userId, excludeIds, requiredGender, myGender, myAge, minBirthDate, maxBirthDate,
            minLat, maxLat, minLon, maxLon, FetchCap);

        // Lọc tinh tuổi + khoảng cách (2 chiều) trước, giữ lại ứng viên hợp lệ
        var passed = new List<(UserProfile Candidate, int Age, double Distance, bool IsBoosted)>();
        foreach (var candidate in candidates)
        {
            // Lọc tinh tuổi (chính xác, không đệm)
            var age = AgeCalculator.FromDateOfBirth(candidate.DateOfBirth);
            if (age is null || age < minAge || age > maxAge)
                continue;

            // Lọc tinh khoảng cách: từ hình vuông -> hình tròn chính xác (bỏ qua nếu tôi chưa có vị trí)
            var distance = hasMyLocation
                ? GeoCalculator.DistanceKm(myLat, myLon, candidate.Latitude!.Value, candidate.Longitude!.Value)
                : 0;
            if (hasMyLocation && distance > maxDistanceKm)
                continue;

            // Lọc 2 chiều khoảng cách: tôi cũng phải nằm trong bán kính họ chấp nhận
            var theirMaxDistance = candidate.User?.Preference?.MaxDistanceKm ?? 50;
            if (hasMyLocation && distance > theirMaxDistance)
                continue;

            var isBoosted = candidate.BoostedUntil.HasValue && candidate.BoostedUntil.Value > now;
            passed.Add((candidate, age.Value, distance, isBoosted));
        }

        // Admin luôn ghim đầu feed — bất kể khoảng cách/tuổi/giới tính, chỉ loại nếu đã bị chặn/đã match
        var admins = await _userRepository.GetAdminsWithProfileAsync();
        var matchedSet = matchedPartners.ToHashSet();
        var blockedSet = blockedRelated.ToHashSet();
        var pinnedAdmins = admins
            .Where(a => a.Id != userId)
            .Where(a => a.Profile is not null && !string.IsNullOrWhiteSpace(a.Profile.DisplayName))
            .Where(a => !blockedSet.Contains(a.Id) && !matchedSet.Contains(a.Id))
            .ToList();
        var adminIds = pinnedAdmins.Select(a => a.Id).ToHashSet();
        passed.RemoveAll(p => adminIds.Contains(p.Candidate.UserId)); // tránh trùng nếu admin cũng lọt qua bộ lọc thường

        // Điểm uy tín theo lô để xếp hạng + gắn badge (fail-safe: lỗi thì coi mọi người là khởi điểm)
        Dictionary<Guid, int> scores;
        try { scores = await _reputationService.GetScoresAsync(passed.Select(p => p.Candidate.UserId).Concat(adminIds)); }
        catch { scores = new Dictionary<Guid, int>(); }

        var adminDtos = pinnedAdmins.Select(a =>
        {
            var adminProfile = a.Profile!;
            var distance = adminProfile.Latitude is not null && adminProfile.Longitude is not null
                ? GeoCalculator.DistanceKm(myLat, myLon, adminProfile.Latitude.Value, adminProfile.Longitude.Value)
                : 0;
            var age = AgeCalculator.FromDateOfBirth(adminProfile.DateOfBirth) ?? 0;
            var score = scores.TryGetValue(a.Id, out var s) ? s : ReputationConfig.StartScore;
            return MapToDto(adminProfile, age, distance, isBoosted: false, ReputationConfig.TierOf(score));
        });

        // Boost lên đầu → rồi uy tín cao hơn → rồi gần hơn
        var rest = passed
            .Select(p =>
            {
                var score = scores.TryGetValue(p.Candidate.UserId, out var s) ? s : ReputationConfig.StartScore;
                var dto = MapToDto(p.Candidate, p.Age, p.Distance, p.IsBoosted, ReputationConfig.TierOf(score));
                return (Dto: dto, Score: score);
            })
            .OrderByDescending(x => x.Dto.IsBoosted)
            .ThenByDescending(x => x.Score)
            .ThenBy(x => x.Dto.DistanceKm)
            .Select(x => x.Dto);

        // Admin luôn ở đầu, bất kể limit — phần còn lại lấp đầy chỗ trống
        return adminDtos.Concat(rest).Take(limit).ToList();
    }

    private DiscoveryProfileDto MapToDto(UserProfile profile, int age, double distanceKm, bool isBoosted, string reputationTier)
    {
        var photos = profile.User?.Photos ?? new List<Photo>();

        return new DiscoveryProfileDto
        {
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            Age = age,
            Gender = profile.Gender,
            Bio = profile.Bio,
            Height = profile.Height,
            Location = profile.Location,
            DatingGoal = profile.DatingGoal,
            DistanceKm = Math.Max(1, (int)Math.Round(distanceKm)), // làm tròn, tối thiểu 1km
            IsBoosted = isBoosted,
            IsPhotoVerified = profile.IsPhotoVerified,
            ReputationTier = reputationTier,
            IsAdmin = profile.User?.Role == UserRole.Admin,
            AvatarFrame = profile.AvatarFrame,
            Photos = _mapper.Map<List<PhotoDto>>(photos.OrderBy(p => p.OrderIndex)),
        };
    }
}
