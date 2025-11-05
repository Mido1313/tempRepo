using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class CreatePriceQuoteRequest
{
    [Required]
    [RegularExpression("^(XAU|XAG)$")]
    public string Symbol { get; set; } = "XAU";

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [Required]
    public decimal PricePerUnit { get; set; }

    [Required]
    public string Source { get; set; } = "manual";
}
