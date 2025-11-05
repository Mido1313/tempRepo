using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class BudgetService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public BudgetService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<IReadOnlyCollection<BudgetDto>> GetAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Budgets
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new BudgetDto(x.Id, x.Name, x.Period, x.Currency, x.Amount, x.StartDate, x.EndDate, x.CreatedUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<BudgetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Budgets
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && x.Id == id)
            .Select(x => new BudgetDto(x.Id, x.Name, x.Period, x.Currency, x.Amount, x.StartDate, x.EndDate, x.CreatedUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<BudgetDto> CreateAsync(UpsertBudgetRequest request, CancellationToken cancellationToken)
    {
        var entity = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = _userContext.UserId,
            Name = request.Name,
            Period = request.Period,
            Currency = request.Currency.ToUpperInvariant(),
            Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero),
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.Budgets.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new BudgetDto(entity.Id, entity.Name, entity.Period, entity.Currency, entity.Amount, entity.StartDate, entity.EndDate, entity.CreatedUtc);
    }

    public async Task<BudgetDto?> UpdateAsync(Guid id, UpsertBudgetRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Budgets.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name;
        entity.Period = request.Period;
        entity.Currency = request.Currency.ToUpperInvariant();
        entity.Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero);
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new BudgetDto(entity.Id, entity.Name, entity.Period, entity.Currency, entity.Amount, entity.StartDate, entity.EndDate, entity.CreatedUtc);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Budgets.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Budgets.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<BudgetProgressDto?> GetProgressAsync(Guid id, DateOnly? from, DateOnly? to, CancellationToken cancellationToken)
    {
        var budget = await _dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);

        if (budget is null)
        {
            return null;
        }

        var start = from ?? budget.StartDate;
        var end = to ?? budget.EndDate;

        var spent = await _dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && x.Currency == budget.Currency && x.BookedOn >= start && x.BookedOn <= end && x.Amount < 0)
            .SumAsync(x => x.Amount, cancellationToken);

        var spentAbs = Math.Abs(decimal.Round(spent, 2, MidpointRounding.AwayFromZero));
        var remaining = decimal.Round(budget.Amount - spentAbs, 2, MidpointRounding.AwayFromZero);

        return new BudgetProgressDto(budget.Id, spentAbs, budget.Amount, remaining, DateTime.UtcNow);
    }
}
