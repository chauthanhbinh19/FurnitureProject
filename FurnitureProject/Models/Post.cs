using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Post : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? ShortDescription { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        // SEO Metadata
        [MaxLength(160)]
        public string? MetaDescription { get; set; }

        [MaxLength(70)]
        public string? MetaTitle { get; set; }

        [MaxLength(100)]
        public string? MetaKeywords { get; set; }

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty; // SEO-friendly URL

        // Other Info
        public string? ThumbnailUrl { get; set; }

        public string Status { get; set; } = "published"; // or "draft"

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        public Guid? AuthorId { get; set; }
        public User? Author { get; set; }

        public ICollection<PostCategoryLink> PostCategoryLinks { get; set; } = new List<PostCategoryLink>();
    }
}
