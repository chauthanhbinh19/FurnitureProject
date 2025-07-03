using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("product_image"); // Tên bảng

            builder.HasKey(u => u.ImageId); // Khóa chính

            builder.Property(u => u.ImageId)
                .ValueGeneratedOnAdd(); // Tự tăng (cho PostgreSQL dùng Identity)

            builder.Property(u => u.ProductId)
                .IsRequired();

            builder.Property(u => u.ImageUrl)
                .IsRequired();

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
