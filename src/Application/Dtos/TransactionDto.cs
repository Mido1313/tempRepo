namespace FinanceZakatManager.Application.Dtos;

public record TransactionDto(
    Guid Id,
    Guid AccountId,
    Guid? CategoryId,
    decimal Amount,
    string Currency,
    DateOnly BookedOn,
    string? Note,
    IReadOnlyCollection<string> Tags,
    DateTime CreatedUtc,
    string? ExternalId,
    bool IsTransfer
);
