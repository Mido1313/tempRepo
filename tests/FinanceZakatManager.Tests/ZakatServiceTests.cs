using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Application.Services;
using FinanceZakatManager.Domain.Entities;
using FinanceZakatManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceZakatManager.Tests;

public class ZakatServiceTests
{
    [Fact]
    public async Task CalculateAsync_Computes_Zakat_When_Above_Nisab()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);
        var userId = Guid.NewGuid();
        context.Accounts.Add(new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = "Cash",
            Currency = "USD",
            OpeningBalance = 10000m,
            CreatedUtc = DateTime.UtcNow,
            IncludeInZakat = true
        });

        context.PriceQuotes.Add(new PriceQuote
        {
            Id = Guid.NewGuid(),
            Symbol = "XAU",
            Currency = "USD",
            PricePerUnit = 60m,
            Source = "test",
            QuotedAtUtc = DateTime.UtcNow,
            CreatedUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        var userContext = new FakeUserContext(userId);
        var priceService = new PriceService(context);
        var service = new ZakatService(context, userContext, priceService);

        var result = await service.CalculateAsync(new ZakatCalculationRequest
        {
            Currency = "USD",
            UseGoldStandard = true
        }, CancellationToken.None);

        Assert.True(result.AmountDue > 0);
        Assert.Equal("USD", result.Currency);
    }

    private sealed class FakeUserContext : IUserContext
    {
        public FakeUserContext(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }

        public string Email => "user@example.com";

        public string? DisplayName => "Test";

        public bool IsInRole(string role) => false;
    }
}
