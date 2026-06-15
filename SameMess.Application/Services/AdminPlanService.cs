using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminPlanService : IAdminPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public AdminPlanService(
        IPlanRepository planRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IAuditService auditService)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _auditService = auditService;
    }

    public async Task<List<AdminPlanDto>> ListAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        return plans.OrderBy(p => p.PriceVnd).Select(ToDto).ToList();
    }

    public async Task<AdminPlanDto> CreateAsync(Guid adminId, PlanPayloadDto dto)
    {
        var existing = await _planRepository.GetByCodeAsync(dto.Code);
        if (existing != null)
            throw new ConflictException($"Mã gói '{dto.Code}' đã tồn tại.");

        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            PriceVnd = dto.PriceVnd,
            DurationDays = dto.DurationDays,
            IsActive = dto.IsActive,
        };
        await _planRepository.AddAsync(plan);
        await _planRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "plan.create", "Plan", plan.Id, plan.Code);
        return ToDto(plan);
    }

    public async Task<AdminPlanDto> UpdateAsync(Guid adminId, Guid id, PlanPayloadDto dto)
    {
        var plan = await _planRepository.GetByIdAsync(id) ?? throw new NotFoundException("Plan", id);
        plan.Name = dto.Name;
        plan.PriceVnd = dto.PriceVnd;
        plan.DurationDays = dto.DurationDays;
        plan.IsActive = dto.IsActive;
        // Code không cho đổi (định danh quyền lợi) — giữ nguyên
        await _planRepository.UpdateAsync(plan);
        await _planRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "plan.update", "Plan", id);
        return ToDto(plan);
    }

    public async Task DeleteAsync(Guid adminId, Guid id)
    {
        var plan = await _planRepository.GetByIdAsync(id) ?? throw new NotFoundException("Plan", id);
        await _planRepository.DeleteAsync(plan);
        await _planRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "plan.delete", "Plan", id, plan.Code);
    }

    public async Task<List<SubscriberDto>> GetSubscribersAsync(string? planCode, string? status)
    {
        var activeOnly = string.Equals(status, "active", StringComparison.OrdinalIgnoreCase);
        var subs = await _subscriptionRepository.GetSubscribersAsync(planCode, activeOnly);
        var users = await _userRepository.GetWithProfileByIdsAsync(subs.Select(s => s.UserId).Distinct().ToList());
        var byId = users.ToDictionary(u => u.Id);
        var now = DateTime.UtcNow;

        return subs.Select(s => new SubscriberDto
        {
            UserId = s.UserId,
            Email = byId.TryGetValue(s.UserId, out var u) ? u.Email : null,
            DisplayName = byId.TryGetValue(s.UserId, out var u2) ? u2.Profile?.DisplayName : null,
            PlanCode = s.PlanCode,
            ExpiresAt = s.ExpiresAt,
            IsActive = s.ExpiresAt > now,
        }).ToList();
    }

    private static AdminPlanDto ToDto(Plan p) => new()
    {
        Id = p.Id,
        Code = p.Code,
        Name = p.Name,
        PriceVnd = p.PriceVnd,
        DurationDays = p.DurationDays,
        IsActive = p.IsActive,
    };
}
