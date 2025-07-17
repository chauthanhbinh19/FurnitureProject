using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.ShortDescription)
                .HasMaxLength(300);

            builder.Property(p => p.Content)
                .IsRequired();

            builder.Property(p => p.MetaTitle)
                .HasMaxLength(70);

            builder.Property(p => p.MetaDescription)
                .HasMaxLength(160);

            builder.Property(p => p.MetaKeywords)
                .HasMaxLength(100);

            builder.Property(p => p.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.ThumbnailUrl);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue("published");

            builder.Property(p => p.PublishedAt)
                .IsRequired();

            builder.Property(p => p.CreatedAt);
            builder.Property(p => p.UpdatedAt);

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

}
