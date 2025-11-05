using System.Security.Cryptography;
using System.Text;
using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class TransactionService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public TransactionService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<IReadOnlyCollection<TransactionDto>> GetAsync(Guid? accountId, Guid? categoryId, DateOnly? from, DateOnly? to, string? q, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId);

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.BookedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.BookedOn <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x => (x.Note ?? string.Empty).Contains(q));
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);

        return await query
            .OrderByDescending(x => x.BookedOn)
            .ThenByDescending(x => x.CreatedUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new TransactionDto(x.Id, x.AccountId, x.CategoryId, x.Amount, x.Currency, x.BookedOn, x.Note, x.Tags, x.CreatedUtc, x.ExternalId, x.IsTransfer))
            .ToListAsync(cancellationToken);
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && x.Id == id)
            .Select(x => new TransactionDto(x.Id, x.AccountId, x.CategoryId, x.Amount, x.Currency, x.BookedOn, x.Note, x.Tags, x.CreatedUtc, x.ExternalId, x.IsTransfer))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TransactionDto> CreateAsync(UpsertTransactionRequest request, CancellationToken cancellationToken)
    {
        var entity = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = _userContext.UserId,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero),
            Currency = request.Currency.ToUpperInvariant(),
            BookedOn = request.BookedOn,
            Note = request.Note,
            Tags = request.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
            ExternalId = request.ExternalId,
            IsTransfer = request.IsTransfer,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.Transactions.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<TransactionDto?> UpdateAsync(Guid id, UpsertTransactionRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.AccountId = request.AccountId;
        entity.CategoryId = request.CategoryId;
        entity.Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero);
        entity.Currency = request.Currency.ToUpperInvariant();
        entity.BookedOn = request.BookedOn;
        entity.Note = request.Note;
        entity.Tags = request.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        entity.ExternalId = request.ExternalId;
        entity.IsTransfer = request.IsTransfer;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Transactions.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> ImportAsync(IEnumerable<UpsertTransactionRequest> rows, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var imported = 0;
        foreach (var row in rows)
        {
            var externalId = row.ExternalId ?? ComputeHash(row);
            var existing = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.UserId == _userContext.UserId && (x.ExternalId == externalId), cancellationToken);
            if (existing != null)
            {
                continue;
            }

            var entity = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = _userContext.UserId,
                AccountId = row.AccountId,
                CategoryId = row.CategoryId,
                Amount = decimal.Round(row.Amount, 2, MidpointRounding.AwayFromZero),
                Currency = row.Currency.ToUpperInvariant(),
                BookedOn = row.BookedOn,
                Note = row.Note,
                Tags = row.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                ExternalId = externalId,
                IsTransfer = row.IsTransfer,
                CreatedUtc = now
            };

            _dbContext.Transactions.Add(entity);
            imported++;
        }

        if (imported > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return imported;
    }

    private static string ComputeHash(UpsertTransactionRequest row)
    {
        var key = string.Join('|', row.AccountId, row.BookedOn, row.Amount, row.Currency.ToUpperInvariant(), row.Note);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(hash);
    }

    private static TransactionDto Map(Transaction entity)
    {
        return new TransactionDto(entity.Id, entity.AccountId, entity.CategoryId, entity.Amount, entity.Currency, entity.BookedOn, entity.Note, entity.Tags, entity.CreatedUtc, entity.ExternalId, entity.IsTransfer);
    }
}
