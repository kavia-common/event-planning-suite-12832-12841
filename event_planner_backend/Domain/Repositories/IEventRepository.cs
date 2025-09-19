using EventPlanner.Domain.Models;

namespace EventPlanner.Domain.Repositories;

/// PUBLIC_INTERFACE
/// <summary>
/// Contract for persisting and retrieving events.
/// </summary>
public interface IEventRepository
{
    Event Add(Event ev);
    Event? GetById(Guid id);
    IEnumerable<Event> GetByOwner(Guid ownerUserId);
    IEnumerable<Event> GetAll();
    Event? Update(Event ev);
    bool Delete(Guid id, Guid ownerUserId);
}
