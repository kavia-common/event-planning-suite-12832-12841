using EventPlanner.Application.DTOs;
using EventPlanner.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Presentation.Controllers;

/// <summary>
/// Guest invitation management for events.
/// </summary>
[Authorize]
[Route("api/events/{eventId:guid}/guests")]
public class GuestsController : ApiControllerBase
{
    private readonly IGuestService _guests;

    public GuestsController(IGuestService guests) => _guests = guests;

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Invite a guest to the specified event (ownership required).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GuestResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [Tags("Guests")]
    public IActionResult Invite([FromRoute] Guid eventId, [FromBody] InviteGuestRequest request)
    {
        try
        {
            var userId = GetUserId();
            var g = _guests.Invite(userId, eventId, request);
            return CreatedAtAction(nameof(List), new { eventId }, ToResponse(g));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// List all guests for an event (ownership required).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GuestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [Tags("Guests")]
    public IActionResult List([FromRoute] Guid eventId)
    {
        try
        {
            var userId = GetUserId();
            var items = _guests.ListByEvent(userId, eventId).Select(ToResponse);
            return Ok(items);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Update a guest RSVP status (ownership required).
    /// </summary>
    [HttpPatch("{guestId:guid}")]
    [ProducesResponseType(typeof(GuestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [Tags("Guests")]
    public IActionResult UpdateStatus([FromRoute] Guid eventId, [FromRoute] Guid guestId, [FromBody] UpdateGuestStatusRequest request)
    {
        var userId = GetUserId();
        var updated = _guests.UpdateStatus(userId, guestId, request);
        if (updated == null) return NotFound();
        return Ok(ToResponse(updated));
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Remove a guest from an event (ownership required).
    /// </summary>
    [HttpDelete("{guestId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Tags("Guests")]
    public IActionResult Remove([FromRoute] Guid eventId, [FromRoute] Guid guestId)
    {
        var userId = GetUserId();
        var ok = _guests.Remove(userId, guestId);
        if (!ok) return NotFound();
        return NoContent();
    }

    private static GuestResponse ToResponse(EventPlanner.Domain.Models.Guest g)
    {
        return new GuestResponse
        {
            Id = g.Id,
            EventId = g.EventId,
            Email = g.Email,
            Name = g.Name,
            Status = g.Status,
            InvitedAtUtc = g.InvitedAtUtc,
            RespondedAtUtc = g.RespondedAtUtc
        };
    }
}
