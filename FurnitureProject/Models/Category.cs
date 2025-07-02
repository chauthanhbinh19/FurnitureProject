using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
