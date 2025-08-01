using Microsoft.AspNetCore.Authorization;

namespace BookstoreApi.Infrastructure.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement
        )
        {
            var hasRole = context
                .User.Claims.Where(c => c.Type == "roles" || c.Type.EndsWith("role"))
                .Any(c => c.Value == requirement.RoleName);

            if (hasRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail(
                    new AuthorizationFailureReason(this, $"Required role: {requirement.RoleName}")
                );
            }

            return Task.CompletedTask;
        }
    }
}
