using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public CategoryType Type { get; set; }
    public string ColorHex { get; set; } = default!;
    public DateTime CreatedUtc { get; set; }

    public UserProfile? User { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
}
