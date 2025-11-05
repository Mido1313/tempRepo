namespace FinanceZakatManager.Domain.Entities;

public class ZakatCalculation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool UsedGoldStandard { get; set; }
    public int HawlDays { get; set; }
    public string Currency { get; set; } = default!;
    public decimal NisabThreshold { get; set; }
    public decimal ZakatableWealth { get; set; }
    public decimal AmountDue { get; set; }
    public DateTime CreatedUtc { get; set; }
}
