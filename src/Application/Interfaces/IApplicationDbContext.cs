using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Account> Accounts { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<PriceQuote> PriceQuotes { get; }
    DbSet<ZakatRule> ZakatRules { get; }
    DbSet<ZakatCalculation> ZakatCalculations { get; }
    DbSet<AuditLog> AuditLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
