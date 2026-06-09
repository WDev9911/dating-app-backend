namespace SameMess.Application.DTOs.Profile;

public class UpdateProfileDto
{
    public string DisplayName { get; set; } = null!;
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Bio { get; set; }
    public int? Height { get; set; }
    public string? Location { get; set; }
    public string? DatingGoal { get; set; }
}
