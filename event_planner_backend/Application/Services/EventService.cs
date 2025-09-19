using EventPlanner.Application.DTOs;
using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Application.Services;

/// PUBLIC_INTERFACE
/// <summary>
/// Event management business rules and operations.
/// </summary>
public interface IEventService
{
    Event Create(Guid ownerUserId, CreateEventRequest request);
    IEnumerable<Event> GetMine(Guid ownerUserId);
    IEnumerable<Event> GetAll();
    Event? GetById(Guid id);
    Event? Update(Guid ownerUserId, Guid id, UpdateEventRequest request);
    bool Delete(Guid ownerUserId, Guid id);
}

public class EventService : IEventService
{
    private readonly IEventRepository _events;
    private readonly IGuestRepository _guests;

    public EventService(IEventRepository events, IGuestRepository guests)
    {
        _events = events;
        _guests = guests;
    }

    public Event Create(Guid ownerUserId, CreateEventRequest request)
    {
        if (request.EndUtc <= request.StartUtc)
            throw new ArgumentException("Event end must be after start.");

        var ev = new Event
        {
            OwnerUserId = ownerUserId,
            Title = request.Title,
            Description = request.Description,
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            Timezone = request.Timezone,
            Location = request.Location,
            Capacity = request.Capacity
        };

        return _events.Add(ev);
    }

    public bool Delete(Guid ownerUserId, Guid id)
    {
        // Optional: also remove guests for this event
        return _events.Delete(id, ownerUserId);
    }

    public IEnumerable<Event> GetAll() => _events.GetAll();

    public Event? GetById(Guid id) => _events.GetById(id);

    public IEnumerable<Event> GetMine(Guid ownerUserId) => _events.GetByOwner(ownerUserId);

    public Event? Update(Guid ownerUserId, Guid id, UpdateEventRequest request)
    {
        var existing = _events.GetById(id);
        if (existing == null || existing.OwnerUserId != ownerUserId) return null;

        if (request.EndUtc <= request.StartUtc)
            throw new ArgumentException("Event end must be after start.");

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.StartUtc = request.StartUtc;
        existing.EndUtc = request.EndUtc;
        existing.Timezone = request.Timezone;
        existing.Location = request.Location;
        existing.Capacity = request.Capacity;

        return _events.Update(existing);
    }
}
