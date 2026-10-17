using Enterprise.Application;
using Enterprise.Infrastructure;
using Enterprise.Persistence;
using Enterprise.Persistence.Context;
using Enterprise.WebAPI.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Clean Architecture Layer Services
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Automatically apply pending database migrations on startup
await app.Services.InitialiseDatabaseAsync();

// Global RFC 7807 Exception Handler Middleware
app.UseExceptionHandler();

// Enable OpenAPI & Scalar API Reference in Development mode
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Enterprise Clean Architecture API");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

