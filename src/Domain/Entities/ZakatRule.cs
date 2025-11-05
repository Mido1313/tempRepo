namespace FinanceZakatManager.Domain.Entities;

public class ZakatRule
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool UseGoldStandard { get; set; }
    public decimal NisabMultiplier { get; set; }
    public DateTime? LastCalculatedUtc { get; set; }
    public DateTime CreatedUtc { get; set; }

    public UserProfile? User { get; set; }
}
