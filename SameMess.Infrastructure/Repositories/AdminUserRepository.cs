using Microsoft.EntityFrameworkCore;
using SameMess.Domain.Entities;
using SameMess.Domain.Interfaces.Repositories;
using SameMess.Infrastructure.Data;

namespace SameMess.Infrastructure.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _context;

    public AdminUserRepository(AppDbContext context) => _context = context;

    public async Task<(List<User> Items, int Total)> SearchAsync(string? search, string? status, int page, int pageSize)
    {
        var query = _context.Users.Include(u => u.Profile).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(u => u.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(u =>
                EF.Functions.ILike(u.Email, pattern)
                || (u.Profile != null && u.Profile.DisplayName != null && EF.Functions.ILike(u.Profile.DisplayName, pattern)));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
