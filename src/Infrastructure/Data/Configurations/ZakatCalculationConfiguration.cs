using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class ZakatCalculationConfiguration : IEntityTypeConfiguration<ZakatCalculation>
{
    public void Configure(EntityTypeBuilder<ZakatCalculation> builder)
    {
        builder.ToTable("zakat_calculations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.CreatedUtc).IsRequired();
    }
}
