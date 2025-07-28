using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Adiciona autenticação com JWT emitido pelo Keycloak
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/bookstore-app";
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,

            RoleClaimType = ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = ctx =>
            {
                var claimsIdentity = ctx.Principal!.Identity as ClaimsIdentity;

                var realmAccessClaim = ctx.Principal.Claims.FirstOrDefault(c =>
                    c.Type == "realm_access"
                );

                if (realmAccessClaim != null)
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);

                    if (
                        doc.RootElement.TryGetProperty("roles", out var rolesElement)
                        && rolesElement.ValueKind == System.Text.Json.JsonValueKind.Array
                    )
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            var roleName = role.GetString();
                            if (!string.IsNullOrEmpty(roleName))
                            {
                                claimsIdentity!.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            }
                        }
                    }
                }

                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API pública - sem login");

app.MapGet(
        "/admin",
        (HttpContext http) =>
        {
            return $"Olá, {http.User.Identity?.Name}! Você acessou a rota /admin";
        }
    )
    .RequireAuthorization(new AuthorizeAttribute { Roles = "admin" });

app.MapGet(
        "/claims",
        (HttpContext http) =>
        {
            return Results.Json(http.User.Claims.Select(c => new { c.Type, c.Value }));
        }
    )
    .RequireAuthorization();

app.Run();
