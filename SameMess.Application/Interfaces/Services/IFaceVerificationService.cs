using SameMess.Application.DTOs.Verification;

namespace SameMess.Application.Interfaces.Services;

/// <summary>
/// Trừu tượng so khớp khuôn mặt. Impl hiện tại không cần native lib (so byte, deterministic).
/// Sau thay bằng DlibFaceVerificationService (FaceRecognitionDotNet) chỉ cần đổi DI.
/// </summary>
public interface IFaceVerificationService
{
    Task<FaceVerificationResult> CompareAsync(Stream selfie, Stream profilePhoto);
}
