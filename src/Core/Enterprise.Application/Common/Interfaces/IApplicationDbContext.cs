using Enterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Abstraction for the Application Database Context.
/// Implemented by Enterprise.Persistence layer.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<StockTransaction> StockTransactions { get; }
    DbSet<Category> Categories { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
