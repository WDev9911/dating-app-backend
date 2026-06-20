namespace SameMess.Application.DTOs.Admin;

public class DashboardStatsDto
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
    public long RevenueVnd { get; set; }              // doanh thu bán gói (subscription)

    // Doanh thu combo hẹn hò (Date Pass)
    public int VoucherOrders { get; set; }            // số voucher đã bán
    public long VoucherGmvVnd { get; set; }           // tổng tiền combo đã bán
    public long VoucherCommissionVnd { get; set; }    // hoa hồng app thu từ voucher
    public long TotalRevenueVnd { get; set; }         // tổng = gói + hoa hồng voucher
}

public class ChartPointDto
{
    public DateOnly Date { get; set; }
    public long Value { get; set; }
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public Guid AdminId { get; set; }
    public string Action { get; set; } = null!;
    public string? TargetType { get; set; }
    public Guid? TargetId { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}
