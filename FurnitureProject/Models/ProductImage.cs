using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class ProductImage : BaseEntity
    {
        public int ImageId { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
