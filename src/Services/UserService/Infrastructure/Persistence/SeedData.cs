using Application.Services;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static void SeedDatabase(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!dbContext.Users.Any()) // Kiểm tra nếu chưa có dữ liệu
        {
            var adminUser = new User(
                id: Guid.NewGuid(),
                username: "admin",
                email: "admin@example.com",
                passwordHash: BcryptStaticPasswordHasher.HashPassword("123456")
            );

            var user1 = new User(
                id: Guid.NewGuid(),
                username: "user1",
                email: "user1@example.com",
                passwordHash: BcryptStaticPasswordHasher.HashPassword("123456")
            );

            dbContext.Users.AddRange(adminUser, user1);
            dbContext.SaveChanges();
        }
    }
}
