using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public AccountType Type { get; set; }
    public string Currency { get; set; } = default!;
    public decimal OpeningBalance { get; set; }
    public DateTime CreatedUtc { get; set; }
    public bool IsArchived { get; set; }
    public bool IncludeInZakat { get; set; }
    public bool IsDebtAccount { get; set; }

    public UserProfile? User { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}
