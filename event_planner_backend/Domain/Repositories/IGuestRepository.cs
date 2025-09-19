using EventPlanner.Domain.Models;

namespace EventPlanner.Domain.Repositories;

/// PUBLIC_INTERFACE
/// <summary>
/// Contract for guest invitation storage and queries.
/// </summary>
public interface IGuestRepository
{
    Guest Add(Guest guest);
    Guest? GetById(Guid id);
    IEnumerable<Guest> GetByEventId(Guid eventId);
    Guest? Update(Guest guest);
    bool Remove(Guid id);
}
