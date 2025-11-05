using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinanceZakatManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = new(
        v => v.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
        v => DateOnly.FromDateTime(DateTime.SpecifyKind(v, DateTimeKind.Utc))
    );

    private static readonly ValueComparer<string[]> TagsComparer = new(
        (a, b) => a != null && b != null && a.SequenceEqual(b),
        v => v is null ? 0 : v.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode(StringComparison.OrdinalIgnoreCase))),
        v => v is null ? Array.Empty<string>() : v.ToArray()
    );

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<PriceQuote> PriceQuotes => Set<PriceQuote>();
    public DbSet<ZakatRule> ZakatRules => Set<ZakatRule>();
    public DbSet<ZakatCalculation> ZakatCalculations => Set<ZakatCalculation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsRelational())
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");
        }
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<Transaction>()
            .Property(x => x.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => string.IsNullOrEmpty(v) ? Array.Empty<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Metadata.SetValueComparer(TagsComparer);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        configurationBuilder.Properties<DateOnly>().HaveConversion(DateOnlyConverter);
        base.ConfigureConventions(configurationBuilder);
    }
}
