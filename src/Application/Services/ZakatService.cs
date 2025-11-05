using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class ZakatService
{
    private const decimal GoldNisabWeight = 85m;
    private const decimal SilverNisabWeight = 595m;

    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;
    private readonly PriceService _priceService;

    public ZakatService(IApplicationDbContext dbContext, IUserContext userContext, PriceService priceService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _priceService = priceService;
    }

    public async Task<ZakatHistoryDto> CalculateAsync(ZakatCalculationRequest request, CancellationToken cancellationToken)
    {
        var currency = request.Currency.ToUpperInvariant();
        var symbol = request.UseGoldStandard ? "XAU" : "XAG";
        var price = await _priceService.GetLatestAsync(symbol, currency, cancellationToken);
        if (price is null)
        {
            throw new InvalidOperationException($"No price quote available for {symbol}/{currency}");
        }

        var accounts = await _dbContext.Accounts
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && x.Currency == currency && !x.IsArchived)
            .Select(x => new { x.Id, x.IncludeInZakat, x.IsDebtAccount, x.OpeningBalance })
            .ToListAsync(cancellationToken);

        var accountIds = accounts.Select(x => x.Id).ToArray();

        var balances = await _dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && accountIds.Contains(x.AccountId) && x.Currency == currency)
            .GroupBy(x => x.AccountId)
            .Select(g => new { g.Key, Total = g.Sum(x => x.Amount) })
            .ToDictionaryAsync(x => x.Key, x => x.Total, cancellationToken);

        decimal zakatable = 0;
        decimal debts = 0;

        foreach (var account in accounts)
        {
            balances.TryGetValue(account.Id, out var movement);
            var balance = account.OpeningBalance + movement;
            if (account.IncludeInZakat && !account.IsDebtAccount)
            {
                zakatable += balance;
            }
            else if (account.IsDebtAccount)
            {
                debts += Math.Abs(balance);
            }
        }

        var netZakatable = Math.Max(0, zakatable - debts);
        var nisab = price.PricePerUnit * (request.UseGoldStandard ? GoldNisabWeight : SilverNisabWeight);
        var amountDue = netZakatable >= nisab ? decimal.Round(netZakatable * 0.025m, 2, MidpointRounding.AwayFromZero) : 0m;

        var calculation = new ZakatCalculation
        {
            Id = Guid.NewGuid(),
            UserId = _userContext.UserId,
            UsedGoldStandard = request.UseGoldStandard,
            HawlDays = request.HawlDays,
            Currency = currency,
            NisabThreshold = decimal.Round(nisab, 2, MidpointRounding.AwayFromZero),
            ZakatableWealth = decimal.Round(netZakatable, 2, MidpointRounding.AwayFromZero),
            AmountDue = amountDue,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.ZakatCalculations.Add(calculation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ZakatHistoryDto(calculation.Id, calculation.UsedGoldStandard, calculation.HawlDays, calculation.Currency, calculation.NisabThreshold, calculation.ZakatableWealth, calculation.AmountDue, calculation.CreatedUtc);
    }

    public async Task<IReadOnlyCollection<ZakatHistoryDto>> GetHistoryAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.ZakatCalculations
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new ZakatHistoryDto(x.Id, x.UsedGoldStandard, x.HawlDays, x.Currency, x.NisabThreshold, x.ZakatableWealth, x.AmountDue, x.CreatedUtc))
            .ToListAsync(cancellationToken);
    }
}
