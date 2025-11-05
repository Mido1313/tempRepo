using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class AccountService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public AccountService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<IReadOnlyCollection<AccountDto>> GetAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId)
            .OrderBy(x => x.Name)
            .Select(x => new AccountDto(x.Id, x.Name, x.Type, x.Currency, x.OpeningBalance, x.IsArchived, x.IncludeInZakat, x.IsDebtAccount, x.CreatedUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId && x.Id == id)
            .Select(x => new AccountDto(x.Id, x.Name, x.Type, x.Currency, x.OpeningBalance, x.IsArchived, x.IncludeInZakat, x.IsDebtAccount, x.CreatedUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var entity = new Account
        {
            Id = Guid.NewGuid(),
            UserId = _userContext.UserId,
            Name = request.Name,
            Type = request.Type,
            Currency = request.Currency.ToUpperInvariant(),
            OpeningBalance = decimal.Round(request.OpeningBalance, 2, MidpointRounding.AwayFromZero),
            CreatedUtc = DateTime.UtcNow,
            IncludeInZakat = request.IncludeInZakat,
            IsDebtAccount = request.IsDebtAccount
        };

        _dbContext.Accounts.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AccountDto(entity.Id, entity.Name, entity.Type, entity.Currency, entity.OpeningBalance, entity.IsArchived, entity.IncludeInZakat, entity.IsDebtAccount, entity.CreatedUtc);
    }

    public async Task<AccountDto?> UpdateAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.Currency = request.Currency.ToUpperInvariant();
        entity.OpeningBalance = decimal.Round(request.OpeningBalance, 2, MidpointRounding.AwayFromZero);
        entity.IsArchived = request.IsArchived;
        entity.IncludeInZakat = request.IncludeInZakat;
        entity.IsDebtAccount = request.IsDebtAccount;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new AccountDto(entity.Id, entity.Name, entity.Type, entity.Currency, entity.OpeningBalance, entity.IsArchived, entity.IncludeInZakat, entity.IsDebtAccount, entity.CreatedUtc);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Accounts.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
