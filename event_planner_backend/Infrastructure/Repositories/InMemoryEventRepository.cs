using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Infrastructure.Repositories;

/// <summary>
/// In-memory event repository with owner scoping.
/// </summary>
public class InMemoryEventRepository : IEventRepository
{
    private readonly List<Event> _events = new();

    public Event Add(Event ev)
    {
        _events.Add(ev);
        return ev;
    }

    public bool Delete(Guid id, Guid ownerUserId)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id && e.OwnerUserId == ownerUserId);
        if (ev == null) return false;
        _events.Remove(ev);
        return true;
    }

    public IEnumerable<Event> GetAll() => _events.OrderByDescending(e => e.StartUtc);

    public Event? GetById(Guid id) => _events.FirstOrDefault(e => e.Id == id);

    public IEnumerable<Event> GetByOwner(Guid ownerUserId) =>
        _events.Where(e => e.OwnerUserId == ownerUserId).OrderByDescending(e => e.StartUtc);

    public Event? Update(Event ev)
    {
        var existing = _events.FindIndex(e => e.Id == ev.Id && e.OwnerUserId == ev.OwnerUserId);
        if (existing < 0) return null;
        _events[existing] = ev;
        return ev;
    }
}
