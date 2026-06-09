using SameMess.Application.DTOs.Verification;
using SameMess.Application.Interfaces.Services;
using SameMess.Application.Verification;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class ProfileVerificationService : IProfileVerificationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPhotoStorageService _photoStorage;
    private readonly IFaceVerificationService _faceVerification;
    private readonly IReputationService _reputationService;

    public ProfileVerificationService(
        IUserRepository userRepository,
        IPhotoStorageService photoStorage,
        IFaceVerificationService faceVerification,
        IReputationService reputationService)
    {
        _userRepository = userRepository;
        _photoStorage = photoStorage;
        _faceVerification = faceVerification;
        _reputationService = reputationService;
    }

    public async Task<VerificationStatusDto> SubmitAsync(Guid userId, Stream selfie, string fileName, string contentType)
    {
        var user = await _userRepository.GetFullProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);
        var profile = user.Profile ?? throw new BadRequestException("Hãy hoàn thiện hồ sơ trước.");

        if (profile.IsPhotoVerified)
            throw new ConflictException("Hồ sơ đã được xác minh.");

        // Cần một ảnh hồ sơ để so khớp (ưu tiên ảnh chính)
        var profilePhoto = user.Photos.FirstOrDefault(p => p.IsPrimary) ?? user.Photos.FirstOrDefault();
        if (profilePhoto is null)
            throw new BadRequestException("Cần ít nhất một ảnh hồ sơ để xác minh.");

        // Đọc selfie vào bộ nhớ (để vừa so khớp vừa có thể lưu lại nếu cần admin xem)
        byte[] selfieBytes;
        using (var ms = new MemoryStream())
        {
            await selfie.CopyToAsync(ms);
            selfieBytes = ms.ToArray();
        }

        var profileStream = await _photoStorage.OpenAsync(profilePhoto.Url)
            ?? throw new BadRequestException("Không đọc được ảnh hồ sơ để so khớp.");

        FaceVerificationResult match;
        await using (profileStream)
        using (var selfieStream = new MemoryStream(selfieBytes))
        {
            match = await _faceVerification.CompareAsync(selfieStream, profileStream);
        }

        // Quyết định theo ngưỡng distance
        string status;
        if (match.Distance < FaceVerificationConfig.AutoApproveBelow)
            status = VerificationStatus.Approved;
        else if (match.Distance > FaceVerificationConfig.AutoRejectAbove)
            status = VerificationStatus.Rejected;
        else
            status = VerificationStatus.Pending;

        // Quyền riêng tư: chỉ lưu selfie khi cần admin xem (Pending). Còn lại không giữ ảnh thô.
        await DeleteSelfieIfAnyAsync(profile);
        if (status == VerificationStatus.Pending)
            profile.VerificationSelfieUrl = await _photoStorage.SaveAsync(
                new MemoryStream(selfieBytes), fileName, contentType);

        await ApplyStatusAsync(profile, status, recordReputation: true);
        await _userRepository.SaveChangesAsync();

        return ToDto(profile);
    }

    public async Task<VerificationStatusDto> GetMyStatusAsync(Guid userId)
    {
        var user = await _userRepository.GetWithProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);
        var profile = user.Profile ?? throw new BadRequestException("Hãy hoàn thiện hồ sơ trước.");
        return ToDto(profile);
    }

    public async Task<List<PendingVerificationDto>> GetPendingAsync()
    {
        var users = await _userRepository.GetPendingFaceVerificationsAsync();
        return users.Select(u => new PendingVerificationDto
        {
            UserId = u.Id,
            DisplayName = u.Profile?.DisplayName ?? string.Empty,
            SelfieUrl = u.Profile?.VerificationSelfieUrl,
            ProfilePhotoUrl = (u.Photos.FirstOrDefault(p => p.IsPrimary) ?? u.Photos.FirstOrDefault())?.Url,
        }).ToList();
    }

    public async Task<VerificationStatusDto> ReviewAsync(Guid userId, bool approve)
    {
        var user = await _userRepository.GetWithProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);
        var profile = user.Profile ?? throw new NotFoundException("Profile", userId);

        if (profile.VerificationStatus != VerificationStatus.Pending)
            throw new BadRequestException("Hồ sơ này không ở trạng thái chờ duyệt.");

        // Quyết định xong thì xóa selfie thô (chỉ giữ cờ kết quả)
        await DeleteSelfieIfAnyAsync(profile);

        var status = approve ? VerificationStatus.Approved : VerificationStatus.Rejected;
        await ApplyStatusAsync(profile, status, recordReputation: true);
        await _userRepository.SaveChangesAsync();

        return ToDto(profile);
    }

    private async Task ApplyStatusAsync(UserProfile profile, string status, bool recordReputation)
    {
        profile.VerificationStatus = status;
        profile.IsPhotoVerified = status == VerificationStatus.Approved;
        profile.UpdatedAt = DateTime.UtcNow;

        // Xác minh thành công → +15 điểm uy tín & GỠ TRẦN 65 (sự kiện 1 lần, fail-safe)
        if (recordReputation && status == VerificationStatus.Approved)
        {
            try { await _reputationService.RecordEventAsync(profile.UserId, ReputationEventType.FaceVerified); }
            catch { /* không để uy tín làm hỏng luồng xác minh */ }
        }
    }

    private async Task DeleteSelfieIfAnyAsync(UserProfile profile)
    {
        if (!string.IsNullOrEmpty(profile.VerificationSelfieUrl))
        {
            await _photoStorage.DeleteAsync(profile.VerificationSelfieUrl);
            profile.VerificationSelfieUrl = null;
        }
    }

    private static VerificationStatusDto ToDto(UserProfile profile) => new()
    {
        Status = profile.VerificationStatus,
        IsPhotoVerified = profile.IsPhotoVerified,
        Message = profile.VerificationStatus switch
        {
            VerificationStatus.Approved => "Đã xác minh khuôn mặt ✓ (+15 uy tín, gỡ trần điểm).",
            VerificationStatus.Pending => "Đang chờ quản trị viên duyệt (ảnh chưa đủ chắc chắn).",
            VerificationStatus.Rejected => "Xác minh không thành công. Hãy thử lại với ảnh rõ mặt hơn.",
            _ => "Chưa xác minh.",
        },
    };
}
