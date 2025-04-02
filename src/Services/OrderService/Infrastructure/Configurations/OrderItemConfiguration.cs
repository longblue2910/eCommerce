using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;


public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.OrderId)
            .IsRequired();

        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Ignore(oi => oi.TotalPrice); // Không lưu TotalPrice vì là computed property

        // ⚡ Thêm index tối ưu truy vấn
        builder.HasIndex(oi => oi.OrderId); // Truy vấn nhanh theo đơn hàng
        builder.HasIndex(oi => oi.ProductId); // Truy vấn theo sản phẩm
    }
}