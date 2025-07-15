using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class PostCategory : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Slug { get; set; }

        public ICollection<PostCategoryLink> PostCategoryLinks { get; set; }
    }
}
