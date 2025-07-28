using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class MaintainerController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "maintainer")]
    public IActionResult Get()
    {
        var user = User.Identity?.Name ?? "desconhecido";
        return Ok($"Olá, {user}! Você acessou o endpoint /v1/maintainer");
    }
}
