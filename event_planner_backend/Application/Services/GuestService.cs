using EventPlanner.Application.DTOs;
using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Application.Services;

/// PUBLIC_INTERFACE
/// <summary>
/// Guest invitation and RSVP logic.
/// </summary>
public interface IGuestService
{
    Guest Invite(Guid ownerUserId, Guid eventId, InviteGuestRequest request);
    IEnumerable<Guest> ListByEvent(Guid ownerUserId, Guid eventId);
    Guest? UpdateStatus(Guid ownerUserId, Guid guestId, UpdateGuestStatusRequest request);
    bool Remove(Guid ownerUserId, Guid guestId);
}

public class GuestService : IGuestService
{
    private readonly IEventRepository _events;
    private readonly IGuestRepository _guests;

    public GuestService(IEventRepository events, IGuestRepository guests)
    {
        _events = events;
        _guests = guests;
    }

    public Guest Invite(Guid ownerUserId, Guid eventId, InviteGuestRequest request)
    {
        var ev = _events.GetById(eventId);
        if (ev == null || ev.OwnerUserId != ownerUserId)
            throw new UnauthorizedAccessException("You do not own this event.");

        var guest = new Guest
        {
            EventId = eventId,
            Email = request.Email,
            Name = request.Name,
            Status = "Pending"
        };

        return _guests.Add(guest);
    }

    public IEnumerable<Guest> ListByEvent(Guid ownerUserId, Guid eventId)
    {
        var ev = _events.GetById(eventId);
        if (ev == null || ev.OwnerUserId != ownerUserId)
            throw new UnauthorizedAccessException("You do not own this event.");

        return _guests.GetByEventId(eventId);
    }

    public bool Remove(Guid ownerUserId, Guid guestId)
    {
        var guest = _guests.GetById(guestId);
        if (guest == null) return false;

        var ev = _events.GetById(guest.EventId);
        if (ev == null || ev.OwnerUserId != ownerUserId)
            return false;

        return _guests.Remove(guestId);
    }

    public Guest? UpdateStatus(Guid ownerUserId, Guid guestId, UpdateGuestStatusRequest request)
    {
        var guest = _guests.GetById(guestId);
        if (guest == null) return null;

        var ev = _events.GetById(guest.EventId);
        if (ev == null || ev.OwnerUserId != ownerUserId)
            return null;

        guest.Status = request.Status;
        guest.RespondedAtUtc = DateTime.UtcNow;
        return _guests.Update(guest);
    }
}
