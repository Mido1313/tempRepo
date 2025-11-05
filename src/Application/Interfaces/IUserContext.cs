namespace FinanceZakatManager.Application.Interfaces;

public interface IUserContext
{
    Guid UserId { get; }
    string Email { get; }
    string? DisplayName { get; }
    bool IsInRole(string role);
}
