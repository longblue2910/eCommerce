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
            logger.LogInformation("🌱 Seeding initial data...");
            SeedDatabase(dbContext, logger);
            logger.LogInformation("✅ Database Seeding Completed.");
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

        List<Permission> permissions =
        [
            new(Guid.NewGuid(), "User.Create", "Tạo người dùng"),
            new(Guid.NewGuid(), "User.Read", "Xem danh sách người dùng"),
            new(Guid.NewGuid(), "User.Update", "Cập nhật thông tin người dùng"),
            new(Guid.NewGuid(), "User.Delete", "Xóa người dùng"),
        
            new(Guid.NewGuid(), "Role.Create", "Tạo vai trò mới"),
            new(Guid.NewGuid(), "Role.Read", "Xem danh sách vai trò"),
            new(Guid.NewGuid(), "Role.Update", "Chỉnh sửa vai trò"),
            new(Guid.NewGuid(), "Role.Delete", "Xóa vai trò"),
        
            new(Guid.NewGuid(), "Permission.Assign", "Gán quyền cho vai trò"),
        ];

        if (!dbContext.Permissions.Any())
        {
            dbContext.Permissions.AddRange(permissions);
            dbContext.SaveChanges();
        }


        if (!dbContext.Permissions.Any())
        {
            dbContext.Permissions.AddRange(permissions);
            dbContext.SaveChanges();
        }

        if (!dbContext.Roles.Any())
        {

            var adminRole = new Role(Guid.NewGuid(), "Admin");
            var managerRole = new Role(Guid.NewGuid(), "Manager");
            var userRole = new Role(Guid.NewGuid(), "User");

            // Gán quyền cho Admin (Toàn quyền)
            adminRole.AddPermission(permissions.First(p => p.Name == "User.Create"));
            adminRole.AddPermission(permissions.First(p => p.Name == "User.Read"));
            adminRole.AddPermission(permissions.First(p => p.Name == "User.Update"));
            adminRole.AddPermission(permissions.First(p => p.Name == "User.Delete"));
            adminRole.AddPermission(permissions.First(p => p.Name == "Role.Create"));
            adminRole.AddPermission(permissions.First(p => p.Name == "Role.Read"));
            adminRole.AddPermission(permissions.First(p => p.Name == "Role.Update"));
            adminRole.AddPermission(permissions.First(p => p.Name == "Role.Delete"));
            adminRole.AddPermission(permissions.First(p => p.Name == "Permission.Assign"));

            // Gán quyền cho Manager (Quản lý người dùng)
            managerRole.AddPermission(permissions.First(p => p.Name == "User.Read"));
            managerRole.AddPermission(permissions.First(p => p.Name == "User.Update"));

            // Gán quyền cho User (Chỉ đọc)
            userRole.AddPermission(permissions.First(p => p.Name == "User.Read"));

            var roles = new List<Role> { adminRole, managerRole, userRole };

            dbContext.Roles.AddRange(roles);
            dbContext.SaveChanges();
        }


        logger.LogInformation("✅ Seeded {Count} users.", 3);
    }
}