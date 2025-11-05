using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.BookedOn });
        builder.HasIndex(x => new { x.UserId, x.AccountId, x.BookedOn });
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.CreatedUtc).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Tags).HasColumnType("text");
        builder.Property(x => x.ExternalId).HasMaxLength(128);
    }
}
