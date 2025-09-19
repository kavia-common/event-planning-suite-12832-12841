namespace EventPlanner.Domain.Models;

/// PUBLIC_INTERFACE
/// <summary>
/// Represents a guest invited to an event and their RSVP state.
/// </summary>
public class Guest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EventId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Status { get; set; } = "Pending"; // Pending | Accepted | Declined
    public DateTime InvitedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAtUtc { get; set; }
}
