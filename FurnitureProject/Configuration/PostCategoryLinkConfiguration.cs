using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class PostCategoryLinkConfiguration : IEntityTypeConfiguration<PostCategoryLink>
    {
        public void Configure(EntityTypeBuilder<PostCategoryLink> builder)
        {
            builder.ToTable("post_category_links");

            builder.HasKey(l => new { l.PostId, l.PostCategoryId }); // Composite Key

            builder.HasOne(l => l.Post)
                .WithMany(p => p.PostCategoryLinks)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.PostCategory)
                .WithMany(c => c.PostCategoryLinks)
                .HasForeignKey(l => l.PostCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
