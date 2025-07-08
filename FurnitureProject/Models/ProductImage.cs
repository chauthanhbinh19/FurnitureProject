using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class ProductImage : BaseEntity
    {
        public Guid ImageId { get; set; } = Guid.NewGuid();

        [Required]
        public string ImageUrl { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
