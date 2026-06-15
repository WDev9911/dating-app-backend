namespace SameMess.Application.DTOs.Search;

/// <summary>Tham số tìm kiếm (bind từ query string). Tất cả tùy chọn.</summary>
public class SearchQueryDto
{
    public string? Gender { get; set; }
    public string? City { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public List<Guid>? Interests { get; set; }
    public int? DistanceKm { get; set; }
    /// <summary>"distance" | "newest" | "age". Mặc định: distance nếu có tọa độ, ngược lại newest.</summary>
    public string? Sort { get; set; }
}
