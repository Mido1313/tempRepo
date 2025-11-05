namespace FinanceZakatManager.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public string KeycloakSub { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public DateTime CreatedUtc { get; set; }
}
