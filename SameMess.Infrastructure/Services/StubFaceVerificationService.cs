using SameMess.Application.DTOs.Verification;
using SameMess.Application.Interfaces.Services;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// So khớp khuôn mặt tạm thời KHÔNG cần native lib: so byte hai ảnh.
/// - Ảnh trùng khít → distance 0 (tự duyệt).
/// - Khác nhau → distance 0.5 → rơi vào vùng "không chắc" → admin duyệt tay.
/// Đủ để chạy & test toàn bộ luồng. Thay bằng DlibFaceVerificationService (FaceRecognitionDotNet)
/// để so khớp thật chỉ cần đổi đăng ký DI — phần còn lại của hệ thống không đổi.
/// </summary>
public class StubFaceVerificationService : IFaceVerificationService
{
    private const double IdenticalDistance = 0.0;
    private const double InconclusiveDistance = 0.5; // nằm giữa ngưỡng → chuyển admin

    public async Task<FaceVerificationResult> CompareAsync(Stream selfie, Stream profilePhoto)
    {
        var selfieBytes = await ReadAllAsync(selfie);
        var profileBytes = await ReadAllAsync(profilePhoto);

        var identical = selfieBytes.AsSpan().SequenceEqual(profileBytes);
        var distance = identical ? IdenticalDistance : InconclusiveDistance;

        return new FaceVerificationResult { IsMatch = identical, Distance = distance };
    }

    private static async Task<byte[]> ReadAllAsync(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}
