using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class ReportService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public ReportService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<ReportSummaryDto> GetSummaryAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.AsNoTracking().Where(x => x.UserId == _userContext.UserId);
        if (from.HasValue)
        {
            query = query.Where(x => x.BookedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookedOn <= to.Value);
        }

        var totals = await query.GroupBy(x => 1).Select(g => new
        {
            Income = g.Where(t => t.Amount > 0).Sum(t => t.Amount),
            Expense = g.Where(t => t.Amount < 0).Sum(t => t.Amount)
        }).FirstOrDefaultAsync(cancellationToken) ?? new { Income = 0m, Expense = 0m };

        var totalIncome = decimal.Round(totals.Income, 2, MidpointRounding.AwayFromZero);
        var totalExpense = decimal.Round(Math.Abs(totals.Expense), 2, MidpointRounding.AwayFromZero);
        var net = decimal.Round(totalIncome - totalExpense, 2, MidpointRounding.AwayFromZero);
        return new ReportSummaryDto(totalIncome, totalExpense, net, DateTime.UtcNow);
    }

    public async Task<CashflowReportDto> GetCashflowAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.AsNoTracking().Where(x => x.UserId == _userContext.UserId);
        if (from.HasValue)
        {
            query = query.Where(x => x.BookedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookedOn <= to.Value);
        }

        var points = await query
            .GroupBy(x => x.BookedOn)
            .OrderBy(g => g.Key)
            .Select(g => new CashflowPoint(g.Key, g.Where(t => t.Amount > 0).Sum(t => t.Amount), Math.Abs(g.Where(t => t.Amount < 0).Sum(t => t.Amount))))
            .ToListAsync(cancellationToken);

        return new CashflowReportDto(points, DateTime.UtcNow);
    }

    public async Task<IReadOnlyCollection<CategorySpendDto>> GetCategorySpendAsync(DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions.AsNoTracking().Where(x => x.UserId == _userContext.UserId && x.CategoryId != null);

        if (from.HasValue)
        {
            query = query.Where(x => x.BookedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookedOn <= to.Value);
        }

        var data = await query
            .GroupBy(x => x.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key!.Value,
                Amount = Math.Abs(g.Where(t => t.Amount < 0).Sum(t => t.Amount)),
                Currency = g.First().Currency
            })
            .Join(_dbContext.Categories.AsNoTracking(), g => g.CategoryId, c => c.Id, (g, c) => new CategorySpendDto(c.Name, decimal.Round(g.Amount, 2, MidpointRounding.AwayFromZero), g.Currency))
            .ToListAsync(cancellationToken);

        return data;
    }
}
