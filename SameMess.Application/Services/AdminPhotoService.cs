using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminPhotoService : IAdminPhotoService
{
    private readonly IPhotoRepository _photoRepository;
    private readonly IAuditService _auditService;

    public AdminPhotoService(IPhotoRepository photoRepository, IAuditService auditService)
    {
        _photoRepository = photoRepository;
        _auditService = auditService;
    }

    public async Task<List<AdminPhotoDto>> ListAsync(string? status)
    {
        var photos = await _photoRepository.GetByStatusAsync(status ?? PhotoStatus.Pending);
        return photos.Select(p => new AdminPhotoDto
        {
            Id = p.Id,
            UserId = p.UserId,
            DisplayName = p.User?.Profile?.DisplayName,
            Url = p.Url,
            Status = p.Status,
            IsPrimary = p.IsPrimary,
            CreatedAt = p.CreatedAt,
        }).ToList();
    }

    public async Task ApproveAsync(Guid adminId, Guid photoId)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId)
            ?? throw new NotFoundException("Photo", photoId);

        photo.Status = PhotoStatus.Approved;
        photo.RejectionReason = null;
        await _photoRepository.UpdateAsync(photo);
        await _photoRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "photo.approve", "Photo", photoId);
    }

    public async Task RejectAsync(Guid adminId, Guid photoId, string? reason)
    {
        var photo = await _photoRepository.GetByIdAsync(photoId)
            ?? throw new NotFoundException("Photo", photoId);

        photo.Status = PhotoStatus.Rejected;
        photo.RejectionReason = reason;
        await _photoRepository.UpdateAsync(photo);
        await _photoRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "photo.reject", "Photo", photoId, reason);
    }
}
