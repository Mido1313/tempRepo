using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
        builder.Property(x => x.Currency).HasMaxLength(3);
        builder.Property(x => x.CreatedUtc).IsRequired();
        builder.Property(x => x.OpeningBalance).HasPrecision(18, 2);
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
