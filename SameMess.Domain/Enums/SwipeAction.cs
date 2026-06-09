namespace SameMess.Domain.Enums;

public static class SwipeAction
{
    public const string Like = "Like";
    public const string Pass = "Pass";
    public const string SuperLike = "SuperLike";

    public static readonly string[] All = { Like, Pass, SuperLike };

    public static bool IsLike(string action) => action is Like or SuperLike;
}
