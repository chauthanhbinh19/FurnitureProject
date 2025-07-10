using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class DiscountCodeConfiguration : IEntityTypeConfiguration<DiscountCode>
    {
        public void Configure(EntityTypeBuilder<DiscountCode> builder)
        {
            builder.ToTable("discount_code"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.Code)
                .IsRequired();

            builder.Property(u => u.DiscountPercent);

            builder.Property(u => u.DiscountAmount);

            builder.Property(u => u.ExpiryDate)
                .IsRequired();

            builder.Property(u => u.UsageLimit)
                .IsRequired();

            builder.Property(u => u.TimeUsed)
                .IsRequired();

            //builder.Property(u => u.IsValid)
            //    .IsRequired();

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
