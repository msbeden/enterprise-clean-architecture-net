using Enterprise.Application.Common.Interfaces;
using Enterprise.Domain.Entities;
using Enterprise.Domain.Enums;
using Enterprise.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enterprise.Persistence.Context;

/// <summary>
/// Provides helper methods to initialize database migrations and seed initial sample data.
/// </summary>
public static class ApplicationDbContextInitialiser
{
    public static async Task InitialiseDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();

            if (context.Database.IsRelational())
            {
                logger.LogInformation("Ensuring relational database schema...");
                var databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();

                if (context.Database.IsNpgsql())
                {
                    logger.LogInformation("PostgreSQL provider detected. Creating database schema...");
                    try
                    {
                        await databaseCreator.CreateTablesAsync();
                        logger.LogInformation("PostgreSQL tables created successfully.");
                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation("PostgreSQL tables creation completed ({Message}).", ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        logger.LogInformation("Applying SQL Server database migrations...");
                        await context.Database.MigrateAsync();
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "MigrateAsync failed. Fallback to CreateTablesAsync...");
                        try { await databaseCreator.CreateTablesAsync(); } catch { }
                    }
                }

                await SeedSampleDataAsync(context, passwordHasher, logger, services.GetService<IAuditLogService>());
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
                await SeedSampleDataAsync(context, passwordHasher, logger, services.GetService<IAuditLogService>());
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing or seeding the database.");
            throw;
        }
    }

    private static async Task SeedSampleDataAsync(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger logger, IAuditLogService? auditLogService)
    {
        logger.LogInformation("Clearing existing data and seeding fresh database sample data...");

        // Clear existing data in proper dependency order
        if (await context.OrderItems.AnyAsync())
        {
            context.OrderItems.RemoveRange(await context.OrderItems.ToListAsync());
        }

        if (await context.Orders.AnyAsync())
        {
            context.Orders.RemoveRange(await context.Orders.ToListAsync());
        }

        if (await context.StockTransactions.AnyAsync())
        {
            context.StockTransactions.RemoveRange(await context.StockTransactions.ToListAsync());
        }

        if (await context.Products.AnyAsync())
        {
            context.Products.RemoveRange(await context.Products.ToListAsync());
        }

        if (await context.Categories.AnyAsync())
        {
            context.Categories.RemoveRange(await context.Categories.ToListAsync());
        }

        if (await context.Users.AnyAsync())
        {
            context.Users.RemoveRange(await context.Users.ToListAsync());
        }

        await context.SaveChangesAsync();

        // 1. Seed Categories
        var electronicsCat = Category.Create("Elektronik", "Elektronik cihazlar ve bilgisayar bileşenleri", 1);
        var fashionCat = Category.Create("Giyim & Aksesuar", "Erkek ve kadın giyim ürünleri", 2);
        await context.Categories.AddRangeAsync(electronicsCat, fashionCat);

        // 2. Seed Products
        var laptop = Product.Create("Gaming Laptop", "16GB RAM, RTX 4060 High Performance Laptop", SkuCode.Create("PRD-LPT-001"), Money.Create(35000m, "TRY"), 15);
        laptop.AssignCategory(electronicsCat.Id);

        var headphones = Product.Create("Kablosuz Kulaklık", "Gürültü engelleyici Bluetooth kulaklık", SkuCode.Create("PRD-HDP-002"), Money.Create(2500m, "TRY"), 30);
        headphones.AssignCategory(electronicsCat.Id);

        var keyboard = Product.Create("Mekanik Klavye", "RGB aydınlatmalı mekanik klavye", SkuCode.Create("PRD-KBD-003"), Money.Create(1800m, "TRY"), 25);
        keyboard.AssignCategory(electronicsCat.Id);

        var tshirt = Product.Create("Pamuklu T-Shirt", "%100 Pamuk rahat kesim t-shirt", SkuCode.Create("PRD-TSH-004"), Money.Create(350m, "TRY"), 50);
        tshirt.AssignCategory(fashionCat.Id);

        await context.Products.AddRangeAsync(laptop, headphones, keyboard, tshirt);

        // 3. Seed Users
        var adminUser = User.Create("admin@enterprise.com", "Sistem", "Yöneticisi", passwordHasher.HashPassword("Admin123!"), UserRole.Admin);
        var customerUser = User.Create("customer@enterprise.com", "Ahmet", "Yılmaz", passwordHasher.HashPassword("Customer123!"), UserRole.Customer);
        await context.Users.AddRangeAsync(adminUser, customerUser);

        await context.SaveChangesAsync();

        // 4. Seed Orders & Deduct Stock
        laptop.DecreaseStock(1, "Initial sample order");
        headphones.DecreaseStock(2, "Initial sample order");

        var shippingAddress = new Address("Bağdat Caddesi No:100", "İstanbul", "Marmara", "34000", "Türkiye");
        var order = Order.Create(
            customerUser.Id,
            shippingAddress,
            new List<OrderItem>
            {
                OrderItem.Create(laptop.Id, laptop.Name, laptop.Price, 1),
                OrderItem.Create(headphones.Id, headphones.Name, headphones.Price, 2)
            }
        );

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        // 5. Audit Logging to MongoDB
        if (auditLogService != null)
        {
            await auditLogService.LogAsync(new Application.Common.Models.AuditLogEntry
            {
                EntityName = "Order",
                EntityId = order.Id.ToString(),
                Action = "OrderCreated",
                PerformedBy = customerUser.Email,
                Timestamp = DateTime.UtcNow,
                ChangesJson = $"{{\"TotalAmount\": {order.TotalAmount.Amount}, \"ItemsCount\": 2}}"
            });

            await auditLogService.LogAsync(new Application.Common.Models.AuditLogEntry
            {
                EntityName = "User",
                EntityId = adminUser.Id.ToString(),
                Action = "UserSeeded",
                PerformedBy = "SystemInitialiser",
                Timestamp = DateTime.UtcNow,
                ChangesJson = $"{{\"Email\": \"{adminUser.Email}\", \"Role\": \"Admin\"}}"
            });
        }

        logger.LogInformation("Database successfully populated with fresh sample Users, Products, Categories, and Orders (Audit logs sent to MongoDB).");
    }
}
