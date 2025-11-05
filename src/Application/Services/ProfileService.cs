using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class ProfileService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public ProfileService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<UserProfile> GetOrCreateAsync(CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(x => x.Id == _userContext.UserId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = new UserProfile
        {
            Id = _userContext.UserId,
            KeycloakSub = _userContext.UserId.ToString(),
            Email = _userContext.Email,
            DisplayName = _userContext.DisplayName ?? _userContext.Email,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }
}
