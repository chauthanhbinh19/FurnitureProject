using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Password)
                .IsRequired();

            builder.Property(u => u.FullName)
                .HasMaxLength(200);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasDefaultValue("user");

            builder.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue("active");

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
