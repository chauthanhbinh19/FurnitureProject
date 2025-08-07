using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class DistrictConfiguration : IEntityTypeConfiguration<District>
    {
        public void Configure(EntityTypeBuilder<District> builder)
        {
            builder.ToTable("districts");

            builder.HasKey(d => d.Code);

            builder.Property(d => d.Code)
                .IsRequired();

            builder.Property(d => d.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.ProvinceCode)
                .IsRequired();

            builder.HasMany(d => d.Wards)
                .WithOne()
                .HasForeignKey(w => w.DistrictCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
