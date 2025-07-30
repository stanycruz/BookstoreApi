using System.Security.Claims;
using BookstoreApi.Application.Services;
using BookstoreApi.Infrastructure.Data;
using BookstoreApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        "Host=localhost;Port=5432;Database=bookstore_db;Username=postgres;Password=82vu7jhw"
    )
);

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
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BookService>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
    );
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
