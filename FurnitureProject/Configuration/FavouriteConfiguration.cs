using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.ToTable("favourites");

        builder.HasKey(f => f.id);

        builder.Property(f => f.userId)
               .IsRequired();

        builder.Property(f => f.productId)
               .IsRequired();

        builder.Property(u => u.CreatedAt);

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(0);

        builder.HasOne(f => f.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(f => f.userId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Product)
            .WithMany(p => p.Favourites)
            .HasForeignKey(f => f.productId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
