namespace FinanceZakatManager.Application.Dtos;

public record CategorySpendDto(
    string Category,
    decimal Amount,
    string Currency
);
