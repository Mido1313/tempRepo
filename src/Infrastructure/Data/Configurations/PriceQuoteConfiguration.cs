using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class PriceQuoteConfiguration : IEntityTypeConfiguration<PriceQuote>
{
    public void Configure(EntityTypeBuilder<PriceQuote> builder)
    {
        builder.ToTable("price_quotes");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Symbol, x.Currency, x.QuotedAtUtc });
        builder.Property(x => x.Symbol).HasMaxLength(3);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.CreatedUtc).IsRequired();
    }
}
