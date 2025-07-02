using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class ProductImage : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
