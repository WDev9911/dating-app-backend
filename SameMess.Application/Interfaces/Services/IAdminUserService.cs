using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Common;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminUserService
{
    Task<PagedResultDto<AdminUserListItemDto>> ListAsync(string? search, string? status, int page, int pageSize);
    Task<AdminUserDetailDto> GetDetailAsync(Guid userId);
    Task<AdminUserDetailDto> UpdateStatusAsync(Guid adminId, Guid userId, UpdateUserStatusDto dto);
    Task<List<AdminNoteDto>> GetNotesAsync(Guid userId);
    Task<AdminNoteDto> AddNoteAsync(Guid adminId, Guid userId, AddNoteDto dto);
    Task ResetPasswordAsync(Guid adminId, Guid userId, AdminResetPasswordDto dto);
    Task RevokeSessionsAsync(Guid adminId, Guid userId);
    Task<BulkActionResultDto> BulkActionAsync(Guid adminId, BulkActionDto dto);
}
