namespace SameMess.Application.DTOs.Billing;

public class EntitlementsDto
{
    public bool UnlimitedLikes { get; set; }
    public int DailyLikeLimit { get; set; }
    public bool CanUndo { get; set; }
    public bool CanBoost { get; set; }
    public bool CanSeeLikedMePhotos { get; set; }
}
