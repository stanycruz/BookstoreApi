using BookstoreApi.Helpers;
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
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);

        if (user is null)
            return Unauthorized("Usuário não encontrado no contexto.");

        return Ok($"Olá, {user.Name}! Você está sincronizado no banco com role '{user.Role}'");
    }
}
