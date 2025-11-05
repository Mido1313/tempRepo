using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceZakatManager.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.UserId, x.Name }).IsUnique();
        builder.Property(x => x.ColorHex).HasMaxLength(7);
        builder.Property(x => x.CreatedUtc).IsRequired();
    }
}
