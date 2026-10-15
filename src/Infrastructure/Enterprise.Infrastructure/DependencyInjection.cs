using Enterprise.Application.Common.Interfaces;
using Enterprise.Infrastructure.Caching;
using Enterprise.Infrastructure.Messaging;
using Enterprise.Infrastructure.Messaging.Consumers;
using Enterprise.Infrastructure.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Infrastructure;

/// <summary>
/// Service collection extension methods for registering Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register General Services
        services.AddTransient<IDateTimeService, DateTimeService>();

        // Register JWT & Security Services
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Register Redis Caching
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "Enterprise_";
        });
        services.AddSingleton<ICacheService, RedisCacheService>();

        // Register MassTransit & RabbitMQ
        services.AddMassTransit(x =>
        {
            // Register Consumers
            x.AddConsumer<ProductCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(host, virtualHost, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        // Register Event Bus
        services.AddScoped<IEventBus, MassTransitEventBus>();

        // Register Audit Log Service (MongoDB)
        services.AddSingleton<IAuditLogService, Enterprise.Infrastructure.Logging.MongoAuditLogService>();

        return services;
    }
}
