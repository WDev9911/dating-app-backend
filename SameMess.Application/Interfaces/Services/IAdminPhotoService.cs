using SameMess.Application.DTOs.Admin;

namespace SameMess.Application.Interfaces.Services;

public interface IAdminPhotoService
{
    Task<List<AdminPhotoDto>> ListAsync(string? status);
    Task ApproveAsync(Guid adminId, Guid photoId);
    Task RejectAsync(Guid adminId, Guid photoId, string? reason);
}
