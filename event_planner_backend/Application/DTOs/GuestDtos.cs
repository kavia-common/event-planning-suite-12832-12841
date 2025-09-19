using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Application.DTOs;

/// PUBLIC_INTERFACE
/// <summary>
/// Guest invite and RSVP DTOs.
/// </summary>
public class InviteGuestRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }
}

public class UpdateGuestStatusRequest
{
    [Required]
    [RegularExpression("Pending|Accepted|Declined")]
    public string Status { get; set; } = "Pending";
}

public class GuestResponse
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime InvitedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
}
