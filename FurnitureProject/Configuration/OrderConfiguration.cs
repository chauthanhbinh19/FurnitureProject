using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.UserId)
                .IsRequired();

            builder.Property(u => u.OrderDate)
                .IsRequired();

            builder.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue("active");

            builder.Property(u => u.TotalAmount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
