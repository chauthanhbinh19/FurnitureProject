using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Data
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags"); // Tên bảng

            builder.HasKey(u => u.Id); // Khóa chính

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

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
