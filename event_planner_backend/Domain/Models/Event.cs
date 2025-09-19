namespace EventPlanner.Domain.Models;

/// PUBLIC_INTERFACE
/// <summary>
/// Represents an event with scheduling and capacity details.
/// </summary>
public class Event
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Timezone { get; set; } = "UTC";
    public string? Location { get; set; }
    public int? Capacity { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsOverlapping(Event other)
    {
        return StartUtc < other.EndUtc && other.StartUtc < EndUtc;
    }
}
