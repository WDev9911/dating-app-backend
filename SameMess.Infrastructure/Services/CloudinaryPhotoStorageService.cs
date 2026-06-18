using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure.Settings;

namespace SameMess.Infrastructure.Services;

/// <summary>
/// Lưu ảnh trên Cloudinary (CDN + tự transform). Bền vĩnh viễn (khác ổ đĩa container).
/// Được chọn khi có Cloudinary:CloudName; nếu không, dùng LocalPhotoStorageService.
/// </summary>
public class CloudinaryPhotoStorageService : IPhotoStorageService
{
    private static readonly HttpClient Http = new();
    private readonly Cloudinary _cloudinary;
    private readonly string _folder;

    public CloudinaryPhotoStorageService(IOptions<CloudinarySettings> options)
    {
        var s = options.Value;
        _cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
        _cloudinary.Api.Secure = true;
        _folder = s.Folder;
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType)
    {
        var result = await _cloudinary.UploadAsync(new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = _folder,
            UniqueFilename = true,
            Overwrite = false,
        });

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }

    public async Task DeleteAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        var publicId = ExtractPublicId(url);
        if (publicId is null) return;

        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }

    public async Task<Stream?> OpenAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        try
        {
            var resp = await Http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadAsStreamAsync();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Trích public id (kèm folder, bỏ đuôi) từ secure URL Cloudinary để xóa.
    /// vd: https://res.cloudinary.com/x/image/upload/v123/samemess/abc.jpg → samemess/abc
    /// </summary>
    private static string? ExtractPublicId(string url)
    {
        const string marker = "/upload/";
        var idx = url.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0) return null;

        var path = url[(idx + marker.Length)..];                     // v123/samemess/abc.jpg
        var slash = path.IndexOf('/');
        if (slash > 1 && path[0] == 'v' && path[1..slash].All(char.IsDigit))
            path = path[(slash + 1)..];                              // samemess/abc.jpg

        var dot = path.LastIndexOf('.');
        if (dot > 0) path = path[..dot];                             // samemess/abc
        return path;
    }
}
