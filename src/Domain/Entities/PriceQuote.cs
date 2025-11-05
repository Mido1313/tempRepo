namespace FinanceZakatManager.Domain.Entities;

public class PriceQuote
{
    public Guid Id { get; set; }
    public string Symbol { get; set; } = default!;
    public decimal PricePerUnit { get; set; }
    public string Currency { get; set; } = default!;
    public string Source { get; set; } = default!;
    public DateTime QuotedAtUtc { get; set; }
    public DateTime CreatedUtc { get; set; }
}
