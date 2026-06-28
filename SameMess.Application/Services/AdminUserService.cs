using SameMess.Application.DTOs.Admin;
using SameMess.Application.DTOs.Common;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminUserService : IAdminUserService
{
    private static readonly HashSet<string> ValidStatuses = new()
    {
        UserStatus.Active, UserStatus.Inactive, UserStatus.Banned, UserStatus.PendingVerification,
    };

    private readonly IUserRepository _userRepository;
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IAdminNoteRepository _noteRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditService _auditService;

    public AdminUserService(
        IUserRepository userRepository,
        IAdminUserRepository adminUserRepository,
        IAdminNoteRepository noteRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _adminUserRepository = adminUserRepository;
        _noteRepository = noteRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _auditService = auditService;
    }

    public async Task<PagedResultDto<AdminUserListItemDto>> ListAsync(string? search, string? status, int page, int pageSize)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var (items, total) = await _adminUserRepository.SearchAsync(search, status, page, pageSize);
        return new PagedResultDto<AdminUserListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Items = items.Select(ToListItem).ToList(),
        };
    }

    public async Task<AdminUserDetailDto> GetDetailAsync(Guid userId)
    {
        var user = await _userRepository.GetWithProfileAsync(userId)
            ?? throw new NotFoundException("User", userId);

        return new AdminUserDetailDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.Profile?.DisplayName,
            Role = user.Role,
            Status = user.Status,
            IsEmailVerified = user.IsEmailVerified,
            IsPhotoVerified = user.Profile?.IsPhotoVerified ?? false,
            CreatedAt = user.CreatedAt,
            PhoneNumber = user.PhoneNumber,
            Gender = user.Profile?.Gender,
            Location = user.Profile?.Location,
            Bio = user.Profile?.Bio,
            VerificationStatus = user.Profile?.VerificationStatus ?? VerificationStatus.None,
            UpdatedAt = user.UpdatedAt,
        };
    }

    public async Task<AdminUserDetailDto> UpdateStatusAsync(Guid adminId, Guid userId, UpdateUserStatusDto dto)
    {
        if (!ValidStatuses.Contains(dto.Status))
            throw new BadRequestException($"Trạng thái không hợp lệ: {dto.Status}");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        user.Status = dto.Status;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        // Nếu khóa tài khoản thì thu hồi mọi phiên đăng nhập
        if (dto.Status is UserStatus.Banned or UserStatus.Inactive)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        await _auditService.LogAsync(adminId, "user.status", "User", userId, $"=> {dto.Status}");
        return await GetDetailAsync(userId);
    }

    public async Task<List<AdminNoteDto>> GetNotesAsync(Guid userId)
    {
        var notes = await _noteRepository.GetByUserIdAsync(userId);
        return notes.Select(ToNoteDto).ToList();
    }

    public async Task<AdminNoteDto> AddNoteAsync(Guid adminId, Guid userId, AddNoteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new BadRequestException("Nội dung ghi chú không được rỗng.");

        var note = new AdminNote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AdminId = adminId,
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
        await _noteRepository.AddAsync(note);
        await _noteRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "user.note", "User", userId);
        return ToNoteDto(note);
    }

    public async Task ResetPasswordAsync(Guid adminId, Guid userId, AdminResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
            throw new BadRequestException("Mật khẩu mới phải có ít nhất 8 ký tự.");

        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();

        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        await _refreshTokenRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "user.reset-password", "User", userId);
    }

    public async Task RevokeSessionsAsync(Guid adminId, Guid userId)
    {
        _ = await _userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
        await _refreshTokenRepository.SaveChangesAsync();

        await _auditService.LogAsync(adminId, "user.revoke-sessions", "User", userId);
    }

    public async Task<BulkActionResultDto> BulkActionAsync(Guid adminId, BulkActionDto dto)
    {
        var ids = dto.UserIds.Distinct().ToList();
        var affected = 0;

        foreach (var id in ids)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) continue;

            switch (dto.Action?.ToLowerInvariant())
            {
                case "ban":
                    user.Status = UserStatus.Banned;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    await _refreshTokenRepository.RevokeAllUserTokensAsync(id);
                    affected++;
                    break;
                case "unban":
                    user.Status = UserStatus.Active;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                    affected++;
                    break;
                case "revoke-sessions":
                    await _refreshTokenRepository.RevokeAllUserTokensAsync(id);
                    affected++;
                    break;
                default:
                    throw new BadRequestException($"Hành động không hợp lệ: {dto.Action}");
            }
        }

        await _userRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "user.bulk", "User", null, $"{dto.Action} x{affected}");
        return new BulkActionResultDto { Affected = affected };
    }

    // Coi là "đang hoạt động" nếu có heartbeat trong vòng 5 phút gần đây.
    private static readonly TimeSpan OnlineWindow = TimeSpan.FromMinutes(5);

    private static AdminUserListItemDto ToListItem(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        DisplayName = u.Profile?.DisplayName,
        Role = u.Role,
        Status = u.Status,
        IsEmailVerified = u.IsEmailVerified,
        IsPhotoVerified = u.Profile?.IsPhotoVerified ?? false,
        CreatedAt = u.CreatedAt,
        LastActiveAt = u.LastActiveAt,
        IsOnline = u.LastActiveAt.HasValue && u.LastActiveAt.Value >= DateTime.UtcNow - OnlineWindow,
    };

    private static AdminNoteDto ToNoteDto(AdminNote n) => new()
    {
        Id = n.Id,
        AdminId = n.AdminId,
        Content = n.Content,
        CreatedAt = n.CreatedAt,
    };
}
