using SameMess.Domain.Enums;

namespace SameMess.Application.Billing;

/// <summary>Quyền lợi suy ra từ gói (nguồn sự thật cho mọi chỗ gate quyền).</summary>
public class PlanEntitlements
{
    public string PlanCode { get; init; } = Domain.Enums.PlanCode.Free;
    public bool UnlimitedLikes { get; init; }
    public int DailyLikeLimit { get; init; }          // chỉ áp dụng khi !UnlimitedLikes
    public bool CanUndo { get; init; }
    public bool CanBoost { get; init; }
    public bool CanSeeLikedMePhotos { get; init; }
}

public static class Entitlements
{
    public const int FreeDailyLikeLimit = 50;

    public static PlanEntitlements For(string planCode) => planCode switch
    {
        PlanCode.Gold => new PlanEntitlements
        {
            PlanCode = PlanCode.Gold,
            UnlimitedLikes = true,
            CanUndo = true,
            CanBoost = true,
            CanSeeLikedMePhotos = true,
        },
        PlanCode.Plus => new PlanEntitlements
        {
            PlanCode = PlanCode.Plus,
            UnlimitedLikes = true,
            CanUndo = true,
            CanBoost = false,
            CanSeeLikedMePhotos = false,
        },
        _ => new PlanEntitlements
        {
            PlanCode = PlanCode.Free,
            UnlimitedLikes = false,
            DailyLikeLimit = FreeDailyLikeLimit,
            CanUndo = false,
            CanBoost = false,
            CanSeeLikedMePhotos = false,
        },
    };
}
