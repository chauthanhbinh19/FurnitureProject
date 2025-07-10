using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class ProductDiscountCodeConfiguration : IEntityTypeConfiguration<ProductDiscountCode>
    {
        public void Configure(EntityTypeBuilder<ProductDiscountCode> builder)
        {
            builder.ToTable("product_discount_code"); // Tên bảng

            builder.Property(u => u.DiscountCodeId)
                .IsRequired();

            builder.Property(u => u.ProductId)
                .IsRequired();

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
