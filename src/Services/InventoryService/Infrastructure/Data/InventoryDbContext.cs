// src/Services/InventoryService/Infrastructure/Data/InventoryDbContext.cs
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InventoryService.Infrastructure.Data;

public class InventoryDbContext : DbContext
{
    public DbSet<Inventory> Inventories { get; set; } = null!;
    public DbSet<InventoryHistory> InventoryHistories { get; set; } = null!;

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Sku).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LocationCode).HasMaxLength(50);
            entity.Property(e => e.LastUpdated).IsRequired();

            // Lưu Dictionary<Guid, int> Reservations dưới dạng JSON
            //entity.Property<string>("ReservationsJson")
            //      .HasColumnName("Reservations")
            //      .HasConversion(
            //          v => JsonSerializer.Serialize(new Dictionary<Guid, int>()),
            //          v => v
            //      );

            // Không lưu thuộc tính này vào DB
            entity.Ignore(e => e.Reservations);
            entity.Ignore(e => e.AvailableQuantity);
        });

        modelBuilder.Entity<InventoryHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();

            entity.HasIndex(e => e.InventoryId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => new { e.InventoryId, e.Timestamp });
        });

        base.OnModelCreating(modelBuilder);
    }
}
