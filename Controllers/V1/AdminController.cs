using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult Get()
    {
        var userName = User.Identity?.Name ?? "desconhecido";
        return Ok($"Olá, {userName}! Você acessou a rota /v1/admin");
    }
}
