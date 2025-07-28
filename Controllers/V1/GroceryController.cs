using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApi.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
public class GroceryController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "grocery")]
    public IActionResult Get()
    {
        var user = User.Identity?.Name ?? "desconhecido";
        return Ok($"Olá, {user}! Você acessou o endpoint /v1/grocery");
    }
}
