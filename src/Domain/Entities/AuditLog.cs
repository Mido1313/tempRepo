namespace FinanceZakatManager.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = default!;
    public string Entity { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public DateTime TimestampUtc { get; set; }
    public string? RequestId { get; set; }
}
