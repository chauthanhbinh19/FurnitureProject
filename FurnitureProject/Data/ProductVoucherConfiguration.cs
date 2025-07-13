using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class ProductVoucherConfiguration : IEntityTypeConfiguration<ProductVoucher>
    {
        public void Configure(EntityTypeBuilder<ProductVoucher> builder)
        {
            builder.ToTable("product_voucher"); // Tên bảng

            builder.Property(u => u.VoucherId)
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
