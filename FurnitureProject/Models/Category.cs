using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FurnitureProject.Models
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "active";
        public ICollection<Product> Products { get; set; }
    }
}
