namespace SameMess.Application.DTOs.Admin;

public class VenuePayloadDto
{
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string? Address { get; set; }
    public string? District { get; set; }
    public string City { get; set; } = "TP.HCM";
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public string? PriceRange { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
