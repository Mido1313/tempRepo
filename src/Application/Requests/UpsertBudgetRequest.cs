using FinanceZakatManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class UpsertBudgetRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public BudgetPeriod Period { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
