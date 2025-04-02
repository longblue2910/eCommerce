using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RefundConfiguration : IEntityTypeConfiguration<Refund>
{
    public void Configure(EntityTypeBuilder<Refund> builder)
    {
        // Đặt tên bảng
        builder.ToTable("Refunds");

        // Khóa chính
        builder.HasKey(r => r.Id);

        // Đánh index cho OrderId để tối ưu truy vấn hoàn tiền theo đơn hàng
        builder.HasIndex(r => r.OrderId);

        // Thuộc tính số tiền hoàn trả
        builder.Property(r => r.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Lưu số tiền dưới dạng decimal(18,2)

        // Enum trạng thái hoàn tiền lưu dưới dạng string
        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        // Ngày yêu cầu hoàn tiền
        builder.Property(r => r.RequestedAt)
            .IsRequired();
    }
}