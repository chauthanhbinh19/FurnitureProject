namespace FurnitureProject.Models
{
    public class Tag : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Status { get; set; } = "active";
        public ICollection<ProductTag> ProductTags { get; set; }
    }
}
