namespace SameMess.Application.Interfaces.Services;

/// <summary>
/// Trừu tượng hóa nơi lưu ảnh. Hiện dùng <c>LocalPhotoStorageService</c> (lưu vào wwwroot);
/// sau này đổi sang cloud chỉ cần thêm một implementation mới và đổi DI, không sửa service/controller.
/// </summary>
public interface IPhotoStorageService
{
    /// <summary>Lưu file và trả về URL tương đối (vd "/uploads/photos/{guid}.jpg").</summary>
    Task<string> SaveAsync(Stream content, string fileName, string contentType);

    /// <summary>Xóa file theo URL tương đối đã lưu. Bỏ qua nếu file không tồn tại.</summary>
    Task DeleteAsync(string url);

    /// <summary>Mở file để đọc (vd so khớp khuôn mặt). Null nếu không tồn tại. Caller chịu trách nhiệm dispose.</summary>
    Task<Stream?> OpenAsync(string url);
}
