using System.Reflection;
using System.Text;
using Enterprise.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Enterprise.WebAPI.Extensions;

/// <summary>
/// Service collection extension methods for registering Presentation (WebAPI) layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Controller Services
        services.AddControllers();

        // 2. Authentication & Authorization
        var secret = configuration["JwtSettings:Secret"] ?? "EnterpriseCleanArchitecture_SecretKey_987654321_ForJwtTokenGeneration!";
        var issuer = configuration["JwtSettings:Issuer"] ?? "EnterpriseCleanArchitecture";
        var audience = configuration["JwtSettings:Audience"] ?? "EnterpriseCleanArchitectureUsers";

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
        });

        services.AddAuthorization();

        // 3. Exception Handling (RFC 7807)
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        // 3. OpenAPI Documentation
        services.AddEndpointsApiExplorer();
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Enterprise Clean Architecture API",
                    Version = "v1",
                    Description = "Production-ready Enterprise .NET Web API built with Clean Architecture, CQRS, Redis & RabbitMQ."
                };

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token into the input field below.\r\n\r\nExample: 'eyJhbGciOiJIUzI1Ni...'"
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes["Bearer"] = securityScheme;

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }] = Array.Empty<string>()
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }
}
