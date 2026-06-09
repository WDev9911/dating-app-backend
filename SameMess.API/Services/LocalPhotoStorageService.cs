using SameMess.Application.Interfaces.Services;

namespace SameMess.API.Services;

/// <summary>
/// Lưu ảnh vào thư mục wwwroot/uploads/photos và phục vụ qua static files.
/// File được đặt tên ngẫu nhiên (GUID) để tránh trùng và chống path traversal.
/// </summary>
public class LocalPhotoStorageService : IPhotoStorageService
{
    private const string RelativeDir = "uploads/photos";
    private readonly IWebHostEnvironment _env;

    public LocalPhotoStorageService(IWebHostEnvironment env) => _env = env;

    private string WebRoot =>
        _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType)
    {
        var ext = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid():N}{ext}";

        var dir = Path.Combine(WebRoot, "uploads", "photos");
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, storedName);
        await using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await content.CopyToAsync(fs);

        return $"/{RelativeDir}/{storedName}";
    }

    public Task DeleteAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.CompletedTask;

        var fullPath = ToFullPath(url);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    public Task<Stream?> OpenAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Task.FromResult<Stream?>(null);

        var fullPath = ToFullPath(url);
        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }

    private string ToFullPath(string url)
    {
        var relative = url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(WebRoot, relative);
    }
}
