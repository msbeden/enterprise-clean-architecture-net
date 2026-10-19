using Enterprise.Application.Common.Interfaces;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Enterprise.IntegrationTests;

public class TestCacheService : ICacheService
{
    private readonly Dictionary<string, object> _cache = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var val) && val is T typedVal)
            return Task.FromResult<T?>(typedVal);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (value != null) _cache[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var val) && val is T typedVal)
            return typedVal;

        var result = await factory(cancellationToken);
        if (result != null) _cache[key] = result;
        return result;
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IEventBus> EventBusMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("UseInMemoryDatabase", "true");

        builder.ConfigureServices(services =>
        {
            // Remove Redis CacheService and replace with TestCacheService
            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICacheService));
            if (cacheDescriptor != null)
            {
                services.Remove(cacheDescriptor);
            }
            services.AddSingleton<ICacheService, TestCacheService>();

            // Remove MassTransit / RabbitMQ EventBus and replace with Mock
            var eventBusDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEventBus));
            if (eventBusDescriptor != null)
            {
                services.Remove(eventBusDescriptor);
            }
            services.AddSingleton(EventBusMock.Object);

            // Remove MassTransit hosted services to prevent RabbitMQ connection attempts
            var hostedServices = services.Where(d => d.ServiceType == typeof(IHostedService)).ToList();
            foreach (var hostedService in hostedServices)
            {
                if (hostedService.ImplementationType?.Assembly.GetName().Name?.Contains("MassTransit") == true)
                {
                    services.Remove(hostedService);
                }
            }
        });
    }

    public HttpClient CreateAuthenticatedClient(string email = "testuser@example.com", UserRole role = UserRole.Customer)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = context.Users.FirstOrDefault(u => u.Email == email.ToLowerInvariant());
        if (user == null)
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            var passwordHash = passwordHasher.HashPassword("Password123!");
            user = User.Create(email, "Test", "User", passwordHash, role);
            context.Users.Add(user);
            context.SaveChangesAsync().GetAwaiter().GetResult();
        }

        var token = tokenGenerator.GenerateToken(user);
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
