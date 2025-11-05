using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class ZakatRuleConfiguration : IEntityTypeConfiguration<ZakatRule>
{
    public void Configure(EntityTypeBuilder<ZakatRule> builder)
    {
        builder.ToTable("zakat_rules");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedUtc).IsRequired();
        builder.Property(x => x.NisabMultiplier).HasPrecision(18, 4);
    }
}
