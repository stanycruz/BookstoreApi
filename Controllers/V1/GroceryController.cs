using BookstoreApi.Helpers;
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
        var user = UserHttpContextHelper.GetCurrentUser(HttpContext);
        return Ok($"Olá, {user?.Name}! Você está autenticado como '{user?.Role}'");
    }
}
