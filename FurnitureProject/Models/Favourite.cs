namespace FurnitureProject.Models
{
    public class Favourite : BaseEntity
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid productId { get; set; }

        public User User { get; set; }
        public Product Product { get; set; }
    }
}
