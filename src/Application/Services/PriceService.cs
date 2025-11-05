using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class PriceService
{
    private readonly IApplicationDbContext _dbContext;

    public PriceService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PriceQuoteDto?> GetLatestAsync(string symbol, string currency, CancellationToken cancellationToken)
    {
        symbol = symbol.ToUpperInvariant();
        currency = currency.ToUpperInvariant();

        return await _dbContext.PriceQuotes
            .AsNoTracking()
            .Where(x => x.Symbol == symbol && x.Currency == currency)
            .OrderByDescending(x => x.QuotedAtUtc)
            .Select(x => new PriceQuoteDto(x.Id, x.Symbol, x.PricePerUnit, x.Currency, x.Source, x.QuotedAtUtc, x.CreatedUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PriceQuoteDto> CreateAsync(CreatePriceQuoteRequest request, CancellationToken cancellationToken)
    {
        var entity = new PriceQuote
        {
            Id = Guid.NewGuid(),
            Symbol = request.Symbol.ToUpperInvariant(),
            Currency = request.Currency.ToUpperInvariant(),
            PricePerUnit = decimal.Round(request.PricePerUnit, 4, MidpointRounding.AwayFromZero),
            Source = request.Source,
            QuotedAtUtc = DateTime.UtcNow,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.PriceQuotes.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new PriceQuoteDto(entity.Id, entity.Symbol, entity.PricePerUnit, entity.Currency, entity.Source, entity.QuotedAtUtc, entity.CreatedUtc);
    }
}
