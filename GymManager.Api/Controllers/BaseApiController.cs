using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace GymManager.Api.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected long GymId
    {
        get
        {
            var gymIdClaim = User.FindFirst("GymId");

            if (gymIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return long.Parse(gymIdClaim.Value);
        }
    }

    protected long UserId
    {
        get
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            return long.Parse(userIdClaim.Value);
        }
    }
}