using SameMess.Application.DTOs.Admin;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class AdminEventService : IAdminEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;

    public AdminEventService(
        IEventRepository eventRepository,
        IEventRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IAuditService auditService)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
        _userRepository = userRepository;
        _auditService = auditService;
    }

    public async Task<List<AdminEventDto>> ListAsync()
    {
        var events = (await _eventRepository.GetAllAsync()).OrderByDescending(e => e.StartAt).ToList();
        var counts = await _registrationRepository.GetCountsByEventsAsync(events.Select(e => e.Id).ToList());
        return events.Select(e => ToDto(e, counts.GetValueOrDefault(e.Id))).ToList();
    }

    public async Task<AdminEventDto> GetAsync(Guid id)
    {
        var e = await _eventRepository.GetByIdAsync(id) ?? throw new NotFoundException("Event", id);
        var count = await _registrationRepository.CountByEventAsync(id);
        return ToDto(e, count);
    }

    public async Task<AdminEventDto> CreateAsync(Guid adminId, EventPayloadDto dto)
    {
        var e = new Event
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            ImageUrl = dto.ImageUrl,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Capacity = dto.Capacity,
            XpReward = dto.XpReward,
            Badge = dto.Badge,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
        };
        await _eventRepository.AddAsync(e);
        await _eventRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "event.create", "Event", e.Id, e.Title);
        return ToDto(e, 0);
    }

    public async Task<AdminEventDto> UpdateAsync(Guid adminId, Guid id, EventPayloadDto dto)
    {
        var e = await _eventRepository.GetByIdAsync(id) ?? throw new NotFoundException("Event", id);
        e.Title = dto.Title;
        e.Description = dto.Description;
        e.Location = dto.Location;
        e.ImageUrl = dto.ImageUrl;
        e.StartAt = dto.StartAt;
        e.EndAt = dto.EndAt;
        e.Capacity = dto.Capacity;
        e.XpReward = dto.XpReward;
        e.Badge = dto.Badge;
        e.UpdatedAt = DateTime.UtcNow;
        await _eventRepository.UpdateAsync(e);
        await _eventRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "event.update", "Event", id);

        var count = await _registrationRepository.CountByEventAsync(id);
        return ToDto(e, count);
    }

    public async Task DeleteAsync(Guid adminId, Guid id)
    {
        var e = await _eventRepository.GetByIdAsync(id) ?? throw new NotFoundException("Event", id);
        await _eventRepository.DeleteAsync(e);
        await _eventRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, "event.delete", "Event", id);
    }

    public async Task<AdminEventDto> SetPublishedAsync(Guid adminId, Guid id, bool published)
    {
        var e = await _eventRepository.GetByIdAsync(id) ?? throw new NotFoundException("Event", id);
        e.IsPublished = published;
        e.UpdatedAt = DateTime.UtcNow;
        await _eventRepository.UpdateAsync(e);
        await _eventRepository.SaveChangesAsync();
        await _auditService.LogAsync(adminId, published ? "event.publish" : "event.unpublish", "Event", id);

        var count = await _registrationRepository.CountByEventAsync(id);
        return ToDto(e, count);
    }

    public async Task<List<EventRegistrationDto>> GetRegistrationsAsync(Guid id)
    {
        var regs = await _registrationRepository.GetByEventAsync(id);
        var users = await _userRepository.GetWithProfileByIdsAsync(regs.Select(r => r.UserId).Distinct().ToList());
        var byId = users.ToDictionary(u => u.Id);

        return regs.Select(r => new EventRegistrationDto
        {
            UserId = r.UserId,
            DisplayName = byId.TryGetValue(r.UserId, out var u) ? u.Profile?.DisplayName : null,
            Status = r.Status,
            RegisteredAt = r.RegisteredAt,
        }).ToList();
    }

    private static AdminEventDto ToDto(Event e, int count) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        Location = e.Location,
        ImageUrl = e.ImageUrl,
        StartAt = e.StartAt,
        EndAt = e.EndAt,
        Capacity = e.Capacity,
        RegisteredCount = count,
        XpReward = e.XpReward,
        Badge = e.Badge,
        IsPublished = e.IsPublished,
        CreatedAt = e.CreatedAt,
    };
}
