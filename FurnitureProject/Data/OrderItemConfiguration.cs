using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_item"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính
            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd(); // Tự tăng (cho PostgreSQL dùng Identity)

            builder.Property(u => u.OrderId)
                .IsRequired();

            builder.Property(u => u.ProductId)
                .IsRequired();

            builder.Property(u => u.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.UnitPrice)
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
