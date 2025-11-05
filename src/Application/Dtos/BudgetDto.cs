using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Application.Dtos;

public record BudgetDto(
    Guid Id,
    string Name,
    BudgetPeriod Period,
    string Currency,
    decimal Amount,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime CreatedUtc
);
