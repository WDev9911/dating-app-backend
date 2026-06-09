namespace SameMess.Application.DTOs.Verification;

/// <summary>Kết quả so khớp selfie với ảnh hồ sơ (do IFaceVerificationService trả về).</summary>
public class FaceVerificationResult
{
    public bool IsMatch { get; set; }
    public double Distance { get; set; } // càng nhỏ càng giống (dlib chuẩn ~0.6)
}
