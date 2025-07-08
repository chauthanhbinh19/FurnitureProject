using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Order : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Status { get; set; } = "Pending"; // hoặc enum nếu muốn

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
