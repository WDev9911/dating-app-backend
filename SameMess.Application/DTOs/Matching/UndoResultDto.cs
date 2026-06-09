namespace SameMess.Application.DTOs.Matching;

public class UndoResultDto
{
    public Guid TargetUserId { get; set; }
    public string Action { get; set; } = null!;
    public bool MatchRemoved { get; set; }
}
