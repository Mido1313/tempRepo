using FinanceZakatManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class CreateAccountRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public AccountType Type { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [Range(typeof(decimal), "-79228162514264337593543950335", "79228162514264337593543950335")]
    public decimal OpeningBalance { get; set; }

    public bool IncludeInZakat { get; set; } = true;

    public bool IsDebtAccount { get; set; }
}
