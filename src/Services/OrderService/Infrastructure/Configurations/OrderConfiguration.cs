using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Aggregates;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<int>() // Lưu Enum dưới dạng int
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        builder.Property(o => o.IsRefunded)
            .IsRequired();

        // ⚡ Thêm index tối ưu truy vấn
        builder.HasIndex(o => o.UserId); // Truy vấn theo User
        builder.HasIndex(o => o.Status); // Truy vấn theo trạng thái đơn hàng
        builder.HasIndex(o => o.CreatedAt); // Truy vấn đơn hàng theo ngày tạo
        builder.HasIndex(o => new { o.UserId, o.Status }); // Truy vấn theo User + Status
    }
}
