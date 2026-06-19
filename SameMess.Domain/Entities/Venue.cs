namespace SameMess.Domain.Entities;

/// <summary>Địa điểm hẹn hò (quán cà phê, nhà hàng...) — gợi ý cho cặp khi cây đạt Level ≥ 4.</summary>
public class Venue
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;   // cafe | restaurant | cinema | park | bar | dessert | other
    public string? Address { get; set; }
    public string? District { get; set; }            // Quận/huyện
    public string City { get; set; } = "TP.HCM";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public string? PriceRange { get; set; }          // $ | $$ | $$$
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
