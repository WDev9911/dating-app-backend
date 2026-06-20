namespace SameMess.Domain.Entities;

/// <summary>Combo khuyến mãi của một quán (1 quán có nhiều combo khác nhau).</summary>
public class VenueCombo
{
    public Guid Id { get; set; }
    public Guid VenueId { get; set; }
    public string Title { get; set; } = null!;          // vd "Combo cà phê đôi"
    public string? Description { get; set; }            // gồm những gì
    public int OriginalPriceVnd { get; set; }           // giá gốc
    public int SalePriceVnd { get; set; }               // giá ưu đãi trong app
    public int CommissionPercent { get; set; } = 15;    // % hoa hồng app thu trên SalePrice
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Venue Venue { get; set; } = null!;
}
