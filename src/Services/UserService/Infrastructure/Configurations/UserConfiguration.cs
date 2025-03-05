using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // ⚡ Định nghĩa bảng Users
        builder.ToTable("Users");

        // ⚡ Khóa chính
        builder.HasKey(u => u.Id);

        // ⚡ Định nghĩa chỉ mục (Index) để tránh trùng Email
        builder.HasIndex(u => u.Email).IsUnique();

        // ⚡ Thuộc tính bắt buộc & độ dài tối đa
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.FullName).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(15);
        builder.Property(u => u.Address).HasMaxLength(255);
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(500);

        // ⚡ Thuộc tính boolean & ngày tháng
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.LastLoginAt).IsRequired(false);


        builder.HasMany(u => u.RefreshTokens) // 1 User có nhiều RefreshTokens
            .WithOne(rt => rt.User)           // 1 RefreshToken thuộc về 1 User
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // ⚡ Quan hệ Many-to-Many với bảng Role (User - Role)
        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<UserRole>(
                j => j.HasOne<Role>().WithMany().HasForeignKey(ur => ur.RoleId),
                j => j.HasOne<User>().WithMany().HasForeignKey(ur => ur.UserId)
            );
    }
}
