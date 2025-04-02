using Domain.Aggregates;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;

namespace Infrastructure.Persistence;

public class OrderDbContext(DbContextOptions<OrderDbContext> options, IMediator mediator) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Refund> Refunds { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
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
