using Microsoft.AspNetCore.Mvc;
using SameMess.Domain.Exceptions;

namespace SameMess.API.Controllers;

[ApiController]
[Produces("application/json", "application/problem+json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Id của user hiện tại lấy từ claim "sub" của JWT.</summary>
    protected Guid CurrentUserId
    {
        get
        {
            var sub = User.FindFirst("sub")?.Value;
            if (!Guid.TryParse(sub, out var userId))
                throw new UnauthorizedException("Invalid token claims.");
            return userId;
        }
    }
}
