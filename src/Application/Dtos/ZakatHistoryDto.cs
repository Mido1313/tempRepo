namespace FinanceZakatManager.Application.Dtos;

public record ZakatHistoryDto(
    Guid Id,
    bool UsedGoldStandard,
    int HawlDays,
    string Currency,
    decimal NisabThreshold,
    decimal ZakatableWealth,
    decimal AmountDue,
    DateTime CreatedUtc
);
