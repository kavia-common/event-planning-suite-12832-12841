using EventPlanner.Application.DTOs;
using EventPlanner.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Presentation.Controllers;

/// <summary>
/// Event lifecycle and scheduling operations.
/// </summary>
[Authorize]
[Route("api/events")]
public class EventsController : ApiControllerBase
{
    private readonly IEventService _events;

    public EventsController(IEventService events) => _events = events;

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Create an event owned by the authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [Tags("Events")]
    public IActionResult Create([FromBody] CreateEventRequest request)
    {
        try
        {
            var userId = GetUserId();
            var ev = _events.Create(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ToResponse(ev));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// List events owned by the authenticated user.
    /// </summary>
    [HttpGet("mine")]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    [Tags("Events")]
    public IActionResult Mine()
    {
        var userId = GetUserId();
        var items = _events.GetMine(userId).Select(ToResponse);
        return Ok(items);
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Retrieve an event by ID. Owner access required for modification actions.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Tags("Events")]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var ev = _events.GetById(id);
        if (ev == null) return NotFound();
        return Ok(ToResponse(ev));
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Update an event the current user owns.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [Tags("Events")]
    public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateEventRequest request)
    {
        try
        {
            var userId = GetUserId();
            var updated = _events.Update(userId, id, request);
            if (updated == null) return NotFound();
            return Ok(ToResponse(updated));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Delete an event owned by the current user.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Tags("Events")]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var userId = GetUserId();
        var ok = _events.Delete(userId, id);
        if (!ok) return NotFound();
        return NoContent();
    }

    private static EventResponse ToResponse(EventPlanner.Domain.Models.Event ev)
    {
        return new EventResponse
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            StartUtc = ev.StartUtc,
            EndUtc = ev.EndUtc,
            Timezone = ev.Timezone,
            Location = ev.Location,
            Capacity = ev.Capacity,
            OwnerUserId = ev.OwnerUserId
        };
    }
}
