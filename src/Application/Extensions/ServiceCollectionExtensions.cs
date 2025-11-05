using FinanceZakatManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceZakatManager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<AccountService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<TransactionService>();
        services.AddScoped<BudgetService>();
        services.AddScoped<PriceService>();
        services.AddScoped<ZakatService>();
        services.AddScoped<ReportService>();
        services.AddScoped<ProfileService>();
        return services;
    }
}
