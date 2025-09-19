namespace EventPlanner.Domain.Models;

/// PUBLIC_INTERFACE
/// <summary>
/// Represents a platform user who can create and manage events.
/// Ocean Professional: Clean model with essential identity fields.
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
