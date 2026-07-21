using AutoMapper;
using SameMess.Application.DTOs.Profile;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ProfileService : IProfileService
{
    private const int MaxPhotosPerUser = 9;
    private const int BoostMinutes = 30;

    private readonly IUserRepository _userRepository;
    private readonly IPhotoRepository _photoRepository;
    private readonly IPhotoStorageService _photoStorage;
    private readonly ITaskService _taskService;
    private readonly IReputationService _reputationService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IReviewService _reviewService;
    private readonly IMapper _mapper;

    public ProfileService(
        IUserRepository userRepository,
        IPhotoRepository photoRepository,
        IPhotoStorageService photoStorage,
        ITaskService taskService,
        IReputationService reputationService,
        ISubscriptionService subscriptionService,
        IReviewService reviewService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _photoRepository = photoRepository;
        _photoStorage = photoStorage;
        _taskService = taskService;
        _reputationService = reputationService;
        _subscriptionService = subscriptionService;
        _reviewService = reviewService;
        _mapper = mapper;
    }

    public async Task<ProfileDto> GetMyProfileAsync(Guid userId)
    {
        var user = await LoadUserAsync(userId);
        var dto = _mapper.Map<ProfileDto>(user);
        await AttachReviewsAsync(dto, userId, userId);
        return dto;
    }

    /// <summary>Hồ sơ công khai của người khác (ẩn preference + toạ độ chính xác).</summary>
    public async Task<ProfileDto> GetPublicProfileAsync(Guid viewerId, Guid userId)
    {
        var user = await LoadUserAsync(userId);
        var dto = _mapper.Map<ProfileDto>(user);
        dto.Preference = null;
        dto.Latitude = null;
        dto.Longitude = null;
        await AttachReviewsAsync(dto, viewerId, userId);
        return dto;
    }

    /// <summary>Gắn điểm sao TB + danh sách review (danh sách chỉ mở cho Gold).</summary>
    private async Task AttachReviewsAsync(ProfileDto dto, Guid viewerId, Guid targetUserId)
    {
        var block = await _reviewService.GetProfileReviewsAsync(viewerId, targetUserId);
        dto.RatingAvg = block.RatingAvg;
        dto.RatingCount = block.RatingCount;
        dto.ReviewsLocked = block.Locked;
        dto.Reviews = block.Reviews;
    }

    public async Task<ProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await LoadUserAsync(userId);
        var profile = user.Profile!;

        profile.DisplayName = dto.DisplayName.Trim();
        profile.Gender = dto.Gender;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.Bio = dto.Bio?.Trim();
        profile.Height = dto.Height;
        profile.Location = dto.Location?.Trim();
        profile.DatingGoal = dto.DatingGoal?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        RecomputeCompleteness(profile, user.Photos.Count);

        await _userRepository.SaveChangesAsync();
        await TryRecordCompleteProfileAsync(userId, profile);
        return _mapper.Map<ProfileDto>(user);
    }

    public async Task<ProfileDto> UpdateLocationAsync(Guid userId, UpdateLocationDto dto)
    {
        var user = await LoadUserAsync(userId);
        var profile = user.Profile!;

        profile.Latitude = dto.Latitude;
        profile.Longitude = dto.Longitude;
        profile.LocationUpdatedAt = DateTime.UtcNow;
        profile.UpdatedAt = DateTime.UtcNow;

        RecomputeCompleteness(profile, user.Photos.Count);

        await _userRepository.SaveChangesAsync();
        await TryRecordCompleteProfileAsync(userId, profile);
        return _mapper.Map<ProfileDto>(user);
    }

    public async Task<PhotoDto> AddPhotoAsync(Guid userId, Stream content, string fileName, string contentType)
    {
        var user = await LoadUserAsync(userId);

        if (user.Photos.Count >= MaxPhotosPerUser)
            throw new ConflictException($"You can upload at most {MaxPhotosPerUser} photos.");

        var url = await _photoStorage.SaveAsync(content, fileName, contentType);
        var isFirst = user.Photos.Count == 0;

        var photo = new Photo
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Url = url,
            OrderIndex = user.Photos.Count,
            IsPrimary = isFirst,
            CreatedAt = DateTime.UtcNow,
        };
        await _photoRepository.AddAsync(photo);

        // Ảnh đầu tiên tự động làm avatar
        if (isFirst)
            user.Profile!.AvatarUrl = url;

        RecomputeCompleteness(user.Profile!, user.Photos.Count + 1);

        await _photoRepository.SaveChangesAsync();
        await TryRecordCompleteProfileAsync(userId, user.Profile!);
        return _mapper.Map<PhotoDto>(photo);
    }

    public async Task DeletePhotoAsync(Guid userId, Guid photoId)
    {
        var user = await LoadUserAsync(userId);
        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new NotFoundException("Photo", photoId);

        await _photoStorage.DeleteAsync(photo.Url);
        await _photoRepository.DeleteAsync(photo);

        var remaining = user.Photos
            .Where(p => p.Id != photoId)
            .OrderBy(p => p.OrderIndex)
            .ToList();

        // Đánh lại thứ tự liên tục cho ảnh còn lại
        for (var i = 0; i < remaining.Count; i++)
            remaining[i].OrderIndex = i;

        // Nếu xóa ảnh primary → chọn ảnh đầu tiên còn lại làm primary
        if (photo.IsPrimary)
        {
            var newPrimary = remaining.FirstOrDefault();
            if (newPrimary != null) newPrimary.IsPrimary = true;
            user.Profile!.AvatarUrl = newPrimary?.Url;
        }

        RecomputeCompleteness(user.Profile!, remaining.Count);

        await _photoRepository.SaveChangesAsync();
    }

    public async Task SetPrimaryPhotoAsync(Guid userId, Guid photoId)
    {
        var user = await LoadUserAsync(userId);
        var target = user.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new NotFoundException("Photo", photoId);

        foreach (var p in user.Photos)
            p.IsPrimary = p.Id == photoId;

        user.Profile!.AvatarUrl = target.Url;
        user.Profile!.UpdatedAt = DateTime.UtcNow;

        await _photoRepository.SaveChangesAsync();
    }

    public async Task ReorderPhotosAsync(Guid userId, ReorderPhotosDto dto)
    {
        var user = await LoadUserAsync(userId);

        var owned = user.Photos.Select(p => p.Id).ToHashSet();
        if (dto.PhotoIds.Distinct().Count() != dto.PhotoIds.Count
            || !dto.PhotoIds.All(owned.Contains)
            || dto.PhotoIds.Count != user.Photos.Count)
        {
            throw new ConflictException("PhotoIds must contain each of your photos exactly once.");
        }

        for (var i = 0; i < dto.PhotoIds.Count; i++)
        {
            var photo = user.Photos.First(p => p.Id == dto.PhotoIds[i]);
            photo.OrderIndex = i;
        }

        await _photoRepository.SaveChangesAsync();
    }

    public async Task<DateTime> BoostAsync(Guid userId)
    {
        // Boost là đặc quyền gói Gold
        var ent = await _subscriptionService.GetEntitlementsAsync(userId);
        if (!ent.CanBoost)
            throw new ForbiddenException("Boost là đặc quyền gói Gold. Nâng cấp để dùng.");

        var user = await LoadUserAsync(userId);
        var until = DateTime.UtcNow.AddMinutes(BoostMinutes);
        user.Profile!.BoostedUntil = until;
        user.Profile.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();
        return until;
    }

    public async Task<ProfileDto> SetAvatarFrameAsync(Guid userId, string? frame)
    {
        var user = await LoadUserAsync(userId);
        if (user.Role != UserRole.Admin)
            throw new ForbiddenException("Chỉ Admin mới đổi được khung hiệu ứng avatar.");
        if (!AvatarFrameType.IsValid(frame))
            throw new BadRequestException("Khung hiệu ứng không hợp lệ.");

        user.Profile!.AvatarFrame = frame;
        user.Profile.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        return _mapper.Map<ProfileDto>(user);
    }

    /// <summary>Ghi nhận thành tựu "hoàn thiện hồ sơ" khi đủ điều kiện (fail-safe, một lần duy nhất).</summary>
    private async Task TryRecordCompleteProfileAsync(Guid userId, UserProfile profile)
    {
        if (!profile.IsProfileCompleted) return;
        try { await _taskService.RecordActionAsync(userId, GameAction.CompleteProfile); }
        catch { /* không để gamification làm hỏng luồng hồ sơ */ }
        try { await _reputationService.RecordEventAsync(userId, ReputationEventType.ProfileCompleted); }
        catch { /* không để uy tín làm hỏng luồng hồ sơ */ }
    }

    private async Task<User> LoadUserAsync(Guid userId) =>
        await _userRepository.GetFullProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);

    /// <summary>
    /// Profile coi là hoàn thiện khi đủ: tên, giới tính, ngày sinh, vị trí và ít nhất 1 ảnh.
    /// Đây là điều kiện tối thiểu để vào Discovery (Phase 2).
    /// </summary>
    private static void RecomputeCompleteness(UserProfile profile, int photoCount)
    {
        profile.IsProfileCompleted =
            !string.IsNullOrWhiteSpace(profile.DisplayName)
            && !string.IsNullOrWhiteSpace(profile.Gender)
            && profile.DateOfBirth.HasValue
            && profile.Latitude.HasValue
            && profile.Longitude.HasValue
            && photoCount > 0;
    }
}
