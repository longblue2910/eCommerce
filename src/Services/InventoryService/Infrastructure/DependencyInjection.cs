// src/Services/InventoryService/Infrastructure/DependencyInjection.cs
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("InventoryDb"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName);
                })
        );

        // Register repositories
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IInventoryHistoryRepository, InventoryHistoryRepository>();

        // Register distributed cache
        //services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = configuration.GetConnectionString("Redis");
        //    options.InstanceName = "InventoryService:";
        //});

        return services;
    }

    // Extension method để initiate database và migrations
    public static IApplicationBuilder UseInventoryInfrastructure(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<InventoryDbContext>>();

        try
        {
            logger.LogInformation("Migrating inventory database");
            dbContext.Database.Migrate();
            logger.LogInformation("Inventory database migration completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while migrating the inventory database");
        }

        return app;
    }
}
