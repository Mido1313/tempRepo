namespace FinanceZakatManager.Application.Dtos;

public record CashflowReportDto(
    IReadOnlyCollection<CashflowPoint> Points,
    DateTime GeneratedUtc
);

public record CashflowPoint(
    DateOnly Date,
    decimal Inflow,
    decimal Outflow
);
