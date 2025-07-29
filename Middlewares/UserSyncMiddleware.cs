using System.Security.Claims;
using BookstoreApi.Application.Services;
using BookstoreApi.Domain.Entities;

namespace BookstoreApi.Middlewares;

public class UserSyncMiddleware
{
    private readonly RequestDelegate _next;

    public UserSyncMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserService userService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userService.GetOrCreateFromClaimsAsync(context.User);
            context.Items["User"] = user;
        }

        await _next(context);
    }
}
