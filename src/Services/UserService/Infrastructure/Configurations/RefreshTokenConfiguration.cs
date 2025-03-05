using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id); // Đặt Id làm khóa chính

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500); // Giới hạn độ dài token

        builder.Property(rt => rt.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()"); // Mặc định là UTC

        builder.Property(rt => rt.ExpiryDate)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(50); // Giới hạn IP

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(50); // Có thể null, giới hạn IP

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500); // Nếu bị thay thế bởi token khác

        // Đánh index cho UserId để tăng tốc truy vấn
        builder.HasIndex(rt => rt.UserId);

        // Nếu có quan hệ với User
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa RefreshToken
    }
}