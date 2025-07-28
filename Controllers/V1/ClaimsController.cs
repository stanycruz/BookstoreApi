using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class ClaimsController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
