using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Description);

            builder.Property(u => u.Price)
                .IsRequired();

            builder.Property(u => u.Stock)
                .IsRequired();

            builder.Property(u => u.CategoryId)
                .IsRequired();

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
