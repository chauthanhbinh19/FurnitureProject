using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
    {
        public void Configure(EntityTypeBuilder<Province> builder)
        {
            builder.ToTable("provinces");

            builder.HasKey(p => p.Code);

            builder.Property(p => p.Code)
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(p => p.Districts)
                .WithOne()
                .HasForeignKey(d => d.ProvinceCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
