namespace SameMess.Application.DTOs.Preference;

public class UpdatePreferenceDto
{
    public string InterestedInGender { get; set; } = null!;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public int MaxDistanceKm { get; set; }
}
