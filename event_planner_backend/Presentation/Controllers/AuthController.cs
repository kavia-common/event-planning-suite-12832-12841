using EventPlanner.Application.DTOs;
using EventPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EventPlanner.Presentation.Controllers;

/// <summary>
/// Authentication endpoints: register and login.
/// Ocean Professional: concise, secure, and clearly documented.
/// </summary>
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">Email, password, and full name.</param>
    /// <returns>Created user basic profile (without sensitive data).</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [Tags("Auth")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        try
        {
            var user = _auth.Register(request);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.CreatedAtUtc
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// PUBLIC_INTERFACE
    /// <summary>
    /// Login using email and password and receive a JWT.
    /// </summary>
    /// <param name="request">Credentials.</param>
    /// <returns>Bearer token and expiry timestamp.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [Tags("Auth")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = _auth.Login(request);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}
