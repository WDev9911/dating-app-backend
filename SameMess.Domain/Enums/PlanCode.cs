namespace SameMess.Domain.Enums;

/// <summary>Gói thuê bao. Free là mặc định (không cần bản ghi Subscription).</summary>
public static class PlanCode
{
    public const string Free = "Free";
    public const string Plus = "Plus";
    public const string Gold = "Gold";

    public static readonly string[] Paid = { Plus, Gold };

    public static bool IsValidPaid(string code) => Paid.Contains(code);
}
