namespace FinanceZakatManager.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
    public DateOnly BookedOn { get; set; }
    public string? Note { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTime CreatedUtc { get; set; }
    public string? ExternalId { get; set; }
    public bool IsTransfer { get; set; }

    public Account? Account { get; set; }
    public Category? Category { get; set; }
    public UserProfile? User { get; set; }
}
