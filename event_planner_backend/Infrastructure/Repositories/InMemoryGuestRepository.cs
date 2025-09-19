using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Infrastructure.Repositories;

/// <summary>
/// In-memory guest repository with simple lookups.
/// </summary>
public class InMemoryGuestRepository : IGuestRepository
{
    private readonly List<Guest> _guests = new();

    public Guest Add(Guest guest)
    {
        _guests.Add(guest);
        return guest;
    }

    public IEnumerable<Guest> GetByEventId(Guid eventId) =>
        _guests.Where(g => g.EventId == eventId).OrderBy(g => g.Email);

    public Guest? GetById(Guid id) => _guests.FirstOrDefault(g => g.Id == id);

    public bool Remove(Guid id)
    {
        var g = _guests.FirstOrDefault(x => x.Id == id);
        if (g == null) return false;
        _guests.Remove(g);
        return true;
    }

    public Guest? Update(Guest guest)
    {
        var idx = _guests.FindIndex(g => g.Id == guest.Id);
        if (idx < 0) return null;
        _guests[idx] = guest;
        return guest;
    }
}
