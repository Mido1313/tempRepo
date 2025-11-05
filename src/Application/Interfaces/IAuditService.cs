namespace FinanceZakatManager.Application.Interfaces;

public interface IAuditService
{
    Task WriteAsync(string action, string entity, string entityId, CancellationToken cancellationToken);
}
