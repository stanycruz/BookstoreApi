using System.Security.Claims;
using BookstoreApi.Domain.Entities;
using BookstoreApi.Infrastructure.Data;

namespace BookstoreApi.Application.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetOrCreateFromClaimsAsync(ClaimsPrincipal claims)
    {
        var sub =
            claims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? claims.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(sub))
            throw new UnauthorizedAccessException("Token sem 'sub' ou 'nameidentifier'.");

        var user = _context.Users.FirstOrDefault(u => u.KeycloakId == sub);
        if (user != null)
            return user;

        var email = claims.FindFirst(ClaimTypes.Email)?.Value ?? "sem-email@local";

        var name =
            claims.FindFirst(ClaimTypes.Name)?.Value
            ?? claims.FindFirst("name")?.Value
            ?? claims.FindFirst("preferred_username")?.Value
            ?? "Desconhecido";

        var role = claims.FindAll(ClaimTypes.Role).FirstOrDefault()?.Value ?? "rookie";

        user = new User
        {
            KeycloakId = sub,
            Email = email,
            Name = name,
            Role = role,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
