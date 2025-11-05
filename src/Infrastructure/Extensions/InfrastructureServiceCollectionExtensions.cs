using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Infrastructure.Data;
using FinanceZakatManager.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceZakatManager.Infrastructure.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default") ?? configuration["DATABASE_URL"] ?? "Host=localhost;Database=finance;Username=finance;Password=finance";

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure();
            });
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<AuditService>();
        services.AddScoped<IAuditService>(sp => sp.GetRequiredService<AuditService>());

        return services;
    }
}
