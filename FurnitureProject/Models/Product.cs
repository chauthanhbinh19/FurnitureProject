using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<ProductPromotion> ProductPromotions { get; set; }
    }
}
