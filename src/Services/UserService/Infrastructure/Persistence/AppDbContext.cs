namespace Infrastructure.Persistence;

using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        // Lấy danh sách các entity có sự kiện miền
        var entitiesWithEvents = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        // Lưu danh sách sự kiện trước khi xóa
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // Xóa sự kiện sau khi lấy
        foreach (var entityEntry in entitiesWithEvents)
        {
            entityEntry.Entity.ClearDomainEvents();
        }

        // Lưu vào database
        var result = await base.SaveChangesAsync(cancellationToken);

        // Publish các sự kiện
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }


}

