using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class ZakatCalculationRequest
{
    public bool UseGoldStandard { get; set; } = true;

    [Range(300, 400)]
    public int HawlDays { get; set; } = 354;

    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";
}
