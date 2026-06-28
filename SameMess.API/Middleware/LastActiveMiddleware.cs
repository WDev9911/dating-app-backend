using Microsoft.EntityFrameworkCore;
using SameMess.Infrastructure.Data;

namespace SameMess.API.Middleware;

/// <summary>
/// Cập nhật mốc hoạt động gần nhất (LastActiveAt) cho user đã đăng nhập ở mỗi request.
/// Có throttle: chỉ ghi DB nếu lần cuối đã quá ngưỡng (mặc định 60s) -> rất nhẹ.
/// </summary>
public class LastActiveMiddleware
{
    private static readonly TimeSpan Throttle = TimeSpan.FromSeconds(60);
    private readonly RequestDelegate _next;

    public LastActiveMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        await _next(context);

        // Chỉ xử lý khi đã xác thực và lấy được userId từ claim "sub"
        if (context.User?.Identity?.IsAuthenticated == true
            && Guid.TryParse(context.User.FindFirst("sub")?.Value, out var userId))
        {
            var now = DateTime.UtcNow;
            var cutoff = now - Throttle;
            try
            {
                // Chỉ UPDATE nếu chưa từng ghi hoặc đã quá ngưỡng throttle (0 row nếu vừa ghi xong)
                await db.Users
                    .Where(u => u.Id == userId && (u.LastActiveAt == null || u.LastActiveAt < cutoff))
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastActiveAt, now));
            }
            catch { /* không để việc ghi heartbeat làm hỏng response */ }
        }
    }
}
