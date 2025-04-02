using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Đặt tên bảng
        builder.ToTable("Payments");

        // Khóa chính
        builder.HasKey(p => p.Id);

        // Đánh index cho OrderId để tối ưu truy vấn
        builder.HasIndex(p => p.OrderId);

        // Thuộc tính số tiền thanh toán
        builder.Property(p => p.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Sử dụng kiểu decimal(18,2) để lưu số tiền

        // Enum trạng thái thanh toán lưu dưới dạng string
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .IsRequired();

        // Enum phương thức thanh toán lưu dưới dạng string
        builder.Property(p => p.Method)
            .HasConversion<string>()
            .IsRequired();

        // Ngày tạo không chỉnh sửa được
        builder.Property(p => p.CreatedAt)
            .IsRequired();
    }
}