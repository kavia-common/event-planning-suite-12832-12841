using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Application.DTOs;

/// PUBLIC_INTERFACE
/// <summary>
/// Event create/update requests and response shapes.
/// </summary>
public class CreateEventRequest
{
    [Required, MinLength(3)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime StartUtc { get; set; }

    [Required]
    public DateTime EndUtc { get; set; }

    [Required, MinLength(2)]
    public string Timezone { get; set; } = "UTC";

    public string? Location { get; set; }
    [Range(1, int.MaxValue)]
    public int? Capacity { get; set; }
}

public class UpdateEventRequest : CreateEventRequest { }

public class EventResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Timezone { get; set; } = "UTC";
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public Guid OwnerUserId { get; set; }
}
