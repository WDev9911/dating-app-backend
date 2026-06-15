using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminInterestService : IAdminInterestService
{
    private readonly IInterestRepository _interestRepository;
    private readonly IAuditService _auditService;

    public AdminInterestService(IInterestRepository interestRepository, IAuditService auditService)
    {
        _interestRepository = interestRepository;
        _auditService = auditService;
    }

    public async Task<List<AdminInterestDto>> ListAsync()
    {
        var items = await _interestRepository.GetAllAsync();
        return items.OrderBy(i => i.GroupName).ThenBy(i => i.Name).Select(ToDto).ToList();
    }

    public async Task<AdminInterestDto> CreateAsync(Guid adminId, InterestPayloadDto dto)
    {
        var interest = new Interest
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            GroupName = dto.GroupName,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
        };
        await _interestRepository.AddAsync(interest);
        await _interestRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "interest.create", "Interest", interest.Id, interest.Name);
        return ToDto(interest);
    }

    public async Task<AdminInterestDto> UpdateAsync(Guid adminId, Guid id, InterestPayloadDto dto)
    {
        var interest = await _interestRepository.GetByIdAsync(id) ?? throw new NotFoundException("Interest", id);
        interest.Name = dto.Name;
        interest.GroupName = dto.GroupName;
        interest.IsActive = dto.IsActive;
        await _interestRepository.UpdateAsync(interest);
        await _interestRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "interest.update", "Interest", id);
        return ToDto(interest);
    }

    public async Task DeleteAsync(Guid adminId, Guid id)
    {
        var interest = await _interestRepository.GetByIdAsync(id) ?? throw new NotFoundException("Interest", id);
        await _interestRepository.DeleteAsync(interest);
        await _interestRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "interest.delete", "Interest", id, interest.Name);
    }

    public async Task<string> CreateGroupAsync(Guid adminId, string name)
    {
        // Nhóm sở thích chỉ là nhãn (GroupName trên Interest), không có bảng riêng.
        // Endpoint này ghi nhận tên nhóm để frontend dùng khi tạo interest.
        await _auditService.LogAsync(adminId, "interest-group.create", "InterestGroup", null, name);
        return name;
    }

    private static AdminInterestDto ToDto(Interest i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        GroupName = i.GroupName,
        IsActive = i.IsActive,
    };
}
