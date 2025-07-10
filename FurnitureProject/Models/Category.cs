using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FurnitureProject.Models
{
    public class Category : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "active";
        public ICollection<Product> Products { get; set; }
    }
}
