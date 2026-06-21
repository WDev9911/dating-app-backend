using SameMess.Application.DTOs.Profile;

namespace SameMess.Application.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileDto> GetMyProfileAsync(Guid userId);
    Task<ProfileDto> GetPublicProfileAsync(Guid userId);
    Task<ProfileDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto);
    Task<ProfileDto> UpdateLocationAsync(Guid userId, UpdateLocationDto dto);

    Task<PhotoDto> AddPhotoAsync(Guid userId, Stream content, string fileName, string contentType);
    Task DeletePhotoAsync(Guid userId, Guid photoId);
    Task SetPrimaryPhotoAsync(Guid userId, Guid photoId);
    Task ReorderPhotosAsync(Guid userId, ReorderPhotosDto dto);

    /// <summary>Kích hoạt Boost: đẩy hồ sơ lên đầu feed người khác trong 30 phút. Trả về thời điểm hết hạn.</summary>
    Task<DateTime> BoostAsync(Guid userId);
}
