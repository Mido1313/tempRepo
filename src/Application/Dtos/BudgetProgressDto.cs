namespace FinanceZakatManager.Application.Dtos;

public record BudgetProgressDto(
    Guid BudgetId,
    decimal Spent,
    decimal Allocated,
    decimal Remaining,
    DateTime GeneratedUtc
);
