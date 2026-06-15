using SameMess.Application.DTOs.Verification;

namespace SameMess.Application.Interfaces.Services;

public interface IProfileVerificationService
{
    /// <summary>User gửi selfie → so khớp với ảnh hồ sơ → tự duyệt / tự từ chối / chờ admin.</summary>
    Task<VerificationStatusDto> SubmitAsync(Guid userId, Stream selfie, string fileName, string contentType);

    Task<VerificationStatusDto> GetMyStatusAsync(Guid userId);

    /// <summary>[Admin] Danh sách hồ sơ chờ duyệt (case so khớp không chắc).</summary>
    Task<List<PendingVerificationDto>> GetPendingAsync();

    /// <summary>[Admin] Chi tiết một hồ sơ xác minh (selfie + ảnh hồ sơ).</summary>
    Task<PendingVerificationDto> GetDetailAsync(Guid userId);

    /// <summary>[Admin] Duyệt/từ chối một hồ sơ đang Pending.</summary>
    Task<VerificationStatusDto> ReviewAsync(Guid userId, bool approve);
}
