namespace SameMess.Domain.Enums;

public static class ReportReason
{
    public const string InappropriateContent = "InappropriateContent";
    public const string Harassment = "Harassment";
    public const string Spam = "Spam";
    public const string FakeProfile = "FakeProfile";
    public const string Underage = "Underage";
    public const string Other = "Other";

    public static readonly string[] All =
        { InappropriateContent, Harassment, Spam, FakeProfile, Underage, Other };
}
