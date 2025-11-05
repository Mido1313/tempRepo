namespace FinanceZakatManager.Application.Dtos;

public record ReportSummaryDto(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal NetCashFlow,
    DateTime GeneratedUtc
);
