using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Application.Services;

public class CategoryService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public CategoryService(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(x => x.UserId == _userContext.UserId)
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Type, x.ColorHex, x.CreatedUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryDto> CreateAsync(UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Id = Guid.NewGuid(),
            UserId = _userContext.UserId,
            Name = request.Name,
            Type = request.Type,
            ColorHex = request.ColorHex.StartsWith('#') ? request.ColorHex : "#" + request.ColorHex,
            CreatedUtc = DateTime.UtcNow
        };

        _dbContext.Categories.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CategoryDto(entity.Id, entity.Name, entity.Type, entity.ColorHex, entity.CreatedUtc);
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.ColorHex = request.ColorHex.StartsWith('#') ? request.ColorHex : "#" + request.ColorHex;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CategoryDto(entity.Id, entity.Name, entity.Type, entity.ColorHex, entity.CreatedUtc);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == _userContext.UserId, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Categories.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
