using BookstoreApi.Domain.Entities;

namespace BookstoreApi.Helpers;

public static class UserHttpContextHelper
{
    public static User? GetCurrentUser(HttpContext context)
    {
        return context.Items["User"] as User;
    }
}
