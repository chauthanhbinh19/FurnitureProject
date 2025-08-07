using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable("wards");

            builder.HasKey(w => w.Code);

            builder.Property(w => w.Code)
                .IsRequired();

            builder.Property(w => w.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(w => w.DistrictCode)
                .IsRequired();
        }
    }
}
