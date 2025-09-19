using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventPlanner.Presentation.Controllers;

/// <summary>
/// Base API controller providing common helpers.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid GetUserId()
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}
