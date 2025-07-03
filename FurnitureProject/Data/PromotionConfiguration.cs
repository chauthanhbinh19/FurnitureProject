using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("promotions"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd(); // Tự tăng (cho PostgreSQL dùng Identity)

            builder.Property(u => u.Title)
                .IsRequired();

            builder.Property(u => u.Description)
                .IsRequired();

            builder.Property(u => u.DiscountPercent)
                .IsRequired();

            builder.Property(u => u.StartDate)
                .IsRequired();

            builder.Property(u => u.EndDate)
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
