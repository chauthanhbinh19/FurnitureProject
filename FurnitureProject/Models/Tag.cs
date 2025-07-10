using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models
{
    public class Tag : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; } = "active";
        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
