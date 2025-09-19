using EventPlanner.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Presentation.Controllers;

/// <summary>
/// User profile endpoints.
/// </summary>
[Authorize]
[Route("api/users")]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Get current authenticated user's profile.
    /// </summary>
    /// <returns>Basic profile fields.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Tags("Users")]
    public IActionResult Me()
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var user = _users.GetById(userId);
        if (user == null) return Unauthorized();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.FullName,
            user.CreatedAtUtc
        });
    }
}
