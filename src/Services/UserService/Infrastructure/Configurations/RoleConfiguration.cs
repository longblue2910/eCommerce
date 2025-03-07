using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        // Thiết lập quan hệ Many-to-Many với Permission
        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions", // Tên bảng trung gian
                j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"));
    }
}
