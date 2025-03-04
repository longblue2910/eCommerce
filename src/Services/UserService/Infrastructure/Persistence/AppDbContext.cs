namespace Infrastructure.Persistence;

using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Common;

public class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
        : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 🔥 Lấy tất cả Domain Events từ các Aggregate Root
        var domainEvents = ChangeTracker.Entries<AggregateRoot<Guid>>() // Lấy tất cả các AggregateRoot có sự thay đổi
            .SelectMany(e => e.Entity.DomainEvents) // Lấy danh sách Domain Events
            .ToList();

        // 🔥 Xóa sự kiện sau khi lấy
        foreach (var entity in ChangeTracker.Entries<AggregateRoot<Guid>>().Select(e => e.Entity))
        {
            entity.ClearDomainEvents();
        }

        // 🔥 Lưu thay đổi vào database
        var result = await base.SaveChangesAsync(cancellationToken);

        // 🔥 Publish tất cả sự kiện (đảm bảo chúng được xử lý)
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}

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
        var users = new List<User>
        {
            new(Guid.NewGuid(), "admin", "admin@example.com", "hashed_password"),
            new(Guid.NewGuid(), "user1", "user1@example.com", "hashed_password")
        };

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        logger.LogInformation("✅ Seeded {Count} users.", users.Count);
    }
}