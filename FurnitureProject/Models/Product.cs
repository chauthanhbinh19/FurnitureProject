using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FurnitureProject.Models
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public string Status { get; set; } = "active";

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<ProductPromotion> ProductPromotions { get; set; }
        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
