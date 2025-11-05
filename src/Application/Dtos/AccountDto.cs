using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Application.Dtos;

public record AccountDto(
    Guid Id,
    string Name,
    AccountType Type,
    string Currency,
    decimal OpeningBalance,
    bool IsArchived,
    bool IncludeInZakat,
    bool IsDebtAccount,
    DateTime CreatedUtc
);
