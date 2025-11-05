using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FinanceZakatManager.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task WriteAsync(string action, string entity, string entityId, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
        var requestId = _httpContextAccessor.HttpContext?.TraceIdentifier;
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = Guid.TryParse(userIdClaim, out var userId) ? userId : null,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            TimestampUtc = DateTime.UtcNow,
            RequestId = requestId
        };

        _dbContext.AuditLogs.Add(log);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
