using SameMess.Application.DTOs.Events;
using SameMess.Application.Interfaces.Services;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;
using SameMess.Domain.Exceptions;
using SameMess.Domain.Interfaces.Repositories;

namespace SameMess.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IEventRegistrationRepository _registrationRepository;

    public EventService(
        IEventRepository eventRepository,
        IEventRegistrationRepository registrationRepository)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task<List<EventDto>> GetEventsAsync(Guid userId)
    {
        var events = await _eventRepository.GetPublishedAsync();
        var ids = events.Select(e => e.Id).ToList();
        var counts = await _registrationRepository.GetCountsByEventsAsync(ids);
        var myRegistered = (await _registrationRepository.GetRegisteredEventIdsAsync(userId)).ToHashSet();

        return events
            .Select(e => ToDto(e, counts.GetValueOrDefault(e.Id), myRegistered.Contains(e.Id)))
            .ToList();
    }

    public async Task<EventDto> GetEventAsync(Guid userId, Guid eventId)
    {
        var e = await _eventRepository.GetByIdAsync(eventId);
        if (e is null || !e.IsPublished)
            throw new NotFoundException("Event", eventId);

        var count = await _registrationRepository.CountByEventAsync(eventId);
        var reg = await _registrationRepository.GetAsync(eventId, userId);
        var isRegistered = reg != null && reg.Status != EventRegistrationStatus.Cancelled;

        return ToDto(e, count, isRegistered);
    }

    public async Task<RegisterResultDto> RegisterAsync(Guid userId, Guid eventId)
    {
        var e = await _eventRepository.GetByIdAsync(eventId);
        if (e is null || !e.IsPublished)
            throw new NotFoundException("Event", eventId);

        var existing = await _registrationRepository.GetAsync(eventId, userId);
        if (existing != null)
        {
            if (existing.Status == EventRegistrationStatus.Cancelled)
            {
                existing.Status = EventRegistrationStatus.Registered;
                existing.RegisteredAt = DateTime.UtcNow;
                await _registrationRepository.UpdateAsync(existing);
                await _registrationRepository.SaveChangesAsync();
            }
            return new RegisterResultDto { RegistrationId = existing.Id, Status = existing.Status };
        }

        if (e.Capacity.HasValue)
        {
            var count = await _registrationRepository.CountByEventAsync(eventId);
            if (count >= e.Capacity.Value)
                throw new BadRequestException("Sự kiện đã đầy chỗ.");
        }

        var registration = new EventRegistration
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            UserId = userId,
            Status = EventRegistrationStatus.Registered,
            RegisteredAt = DateTime.UtcNow,
        };
        await _registrationRepository.AddAsync(registration);
        await _registrationRepository.SaveChangesAsync();

        return new RegisterResultDto { RegistrationId = registration.Id, Status = registration.Status };
    }

    public async Task<List<EventHistoryDto>> GetHistoryAsync(Guid userId)
    {
        var regs = await _registrationRepository.GetUserRegistrationsAsync(userId);
        return regs.Select(r => new EventHistoryDto
        {
            EventId = r.EventId,
            Title = r.Event.Title,
            StartAt = r.Event.StartAt,
            Status = r.Status,
            RegisteredAt = r.RegisteredAt,
            XpEarned = r.Status == EventRegistrationStatus.Attended ? r.Event.XpReward : 0,
            Badge = r.Status == EventRegistrationStatus.Attended ? r.Event.Badge : null,
        }).ToList();
    }

    public async Task<EventRewardDto> GetRewardAsync(Guid eventId)
    {
        var e = await _eventRepository.GetByIdAsync(eventId)
            ?? throw new NotFoundException("Event", eventId);

        return new EventRewardDto { Xp = e.XpReward, Badge = e.Badge };
    }

    private static EventDto ToDto(Event e, int registeredCount, bool isRegistered) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        Location = e.Location,
        ImageUrl = e.ImageUrl,
        StartAt = e.StartAt,
        EndAt = e.EndAt,
        Capacity = e.Capacity,
        RegisteredCount = registeredCount,
        XpReward = e.XpReward,
        Badge = e.Badge,
        IsRegistered = isRegistered,
    };
}
