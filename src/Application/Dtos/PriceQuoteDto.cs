namespace FinanceZakatManager.Application.Dtos;

public record PriceQuoteDto(
    Guid Id,
    string Symbol,
    decimal PricePerUnit,
    string Currency,
    string Source,
    DateTime QuotedAtUtc,
    DateTime CreatedUtc
);
