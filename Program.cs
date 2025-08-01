using System.Security.Claims;
using BookstoreApi.Application.Services;
using BookstoreApi.Infrastructure.Authorization;
using BookstoreApi.Infrastructure.Data;
using BookstoreApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "RequireAdmin",
        policy => policy.Requirements.Add(new RoleRequirement("admin"))
    );

    options.AddPolicy(
        "RequireOwner",
        policy => policy.Requirements.Add(new RoleRequirement("owner"))
    );

    options.AddPolicy(
        "RequireGrocery",
        policy => policy.Requirements.Add(new RoleRequirement("grocery"))
    );

    options.AddPolicy(
        "RequireMaintainer",
        policy => policy.Requirements.Add(new RoleRequirement("maintainer"))
    );

    options.AddPolicy(
        "RequireRookie",
        policy => policy.Requirements.Add(new RoleRequirement("rookie"))
    );
});

builder.Services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();

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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Documentação básica
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Relatórios API",
            Version = "v1",
            Description = "API para geração e controle de relatórios com autenticação via Keycloak",
        }
    );

    // Autenticação via Bearer Token
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT como: Bearer {seu_token}",
        }
    );

    // Aplica o esquema de segurança globalmente
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>();
app.UseAuthorization();

app.MapControllers();

if (
    app.Environment.IsDevelopment()
    || app.Environment.IsStaging()
    || app.Environment.IsProduction()
)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Relatórios API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.Run();
