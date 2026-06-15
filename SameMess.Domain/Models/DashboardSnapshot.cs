namespace SameMess.Domain.Models;

/// <summary>Số liệu tổng quan cho dashboard admin (trả từ repository thống kê).</summary>
public class DashboardSnapshot
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int BannedUsers { get; set; }
    public int PendingVerifications { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsers7Days { get; set; }
    public int TotalMatches { get; set; }
    public int PendingReports { get; set; }
    public int ActiveSubscriptions { get; set; }
    public long RevenueVnd { get; set; }
}

public class DailyCount
{
    public DateOnly Date { get; set; }
    public long Value { get; set; }
}
