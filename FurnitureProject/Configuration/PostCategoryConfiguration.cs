using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class PostCategoryConfiguration : IEntityTypeConfiguration<PostCategory>
    {
        public void Configure(EntityTypeBuilder<PostCategory> builder)
        {
            builder.ToTable("post_categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Slug)
                .HasMaxLength(200);

            builder.Property(c => c.CreatedAt);
            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }

}
