using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class UpsertTransactionRequest
{
    [Required]
    public Guid AccountId { get; set; }

    public Guid? CategoryId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";

    [Required]
    public DateOnly BookedOn { get; set; }

    public string? Note { get; set; }

    public List<string> Tags { get; set; } = new();

    public string? ExternalId { get; set; }

    public bool IsTransfer { get; set; }
}
