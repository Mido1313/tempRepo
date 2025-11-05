using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Domain.Entities;

public class Budget
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public BudgetPeriod Period { get; set; }
    public string Currency { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedUtc { get; set; }

    public UserProfile? User { get; set; }
}
