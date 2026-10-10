using Enterprise.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Persistence.Configurations;

/// <summary>
/// EF Core Entity Configuration for StockTransaction entity.
/// </summary>
public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.ToTable("StockTransactions");

        builder.HasKey(st => st.Id);

        builder.Property(st => st.ProductId)
            .IsRequired();

        builder.Property(st => st.Quantity)
            .IsRequired();

        builder.Property(st => st.TransactionType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(st => st.Reason)
            .HasMaxLength(500);

        builder.Property(st => st.TransactionDateUtc)
            .IsRequired();
    }
}
