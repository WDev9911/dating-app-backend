using SameMess.Domain.Entities;

namespace SameMess.Domain.Interfaces.Repositories;

public interface IAdminUserRepository
{
    /// <summary>Tìm user (kèm Profile) phân trang, lọc theo search (email/tên) + status.</summary>
    Task<(List<User> Items, int Total)> SearchAsync(string? search, string? status, int page, int pageSize);
}
