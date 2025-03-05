using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class MigrationManager
{
    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<AppDbContext>();

        // ✅ Sử dụng ILoggerFactory để tạo logger
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MigrationManager");

        try
        {
            // 🛠 Thực hiện Migration
            logger.LogInformation("🔄 Applying Database Migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("✅ Database Migrations Applied Successfully.");

            // 🛠 Seed Data (nếu cần)
            if (!dbContext.Users.Any())
            {
                logger.LogInformation("🌱 Seeding initial data...");
                SeedDatabase(dbContext, logger);
                logger.LogInformation("✅ Database Seeding Completed.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Migration Failed!");
            throw;
        }
    }

    private static void SeedDatabase(AppDbContext dbContext, ILogger logger)
    {
        if (!dbContext.Users.Any())
        {
            var adminUser = new User(
                id: Guid.NewGuid(),
                username: "admin",
                email: "admin@example.com",
                passwordHash: BcryptStaticPasswordHasher.HashPassword("Admin@123"),
                fullName: "Administrator",
                phoneNumber: "0123456789"
            );

            var user1 = new User(
                id: Guid.NewGuid(),
                username: "user1",
                email: "user1@example.com",
                passwordHash: BcryptStaticPasswordHasher.HashPassword("User@123"),
                fullName: "User One",
                phoneNumber: "0987654321"
            );

            var user2 = new User(
                id: Guid.NewGuid(),
                username: "user2",
                email: "user2@example.com",
                passwordHash: BcryptStaticPasswordHasher.HashPassword("User@123"),
                fullName: "User Two",
                phoneNumber: "0912345678"
            );

            dbContext.Users.AddRange(adminUser, user1, user2);
            dbContext.SaveChanges();
        }


        logger.LogInformation("✅ Seeded {Count} users.", 3);
    }
}