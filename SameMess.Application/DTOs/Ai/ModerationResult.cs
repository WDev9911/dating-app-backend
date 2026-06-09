namespace SameMess.Application.DTOs.Ai;

public class ModerationResult
{
    public bool IsFlagged { get; set; }
    public string[] Categories { get; set; } = Array.Empty<string>();
}
