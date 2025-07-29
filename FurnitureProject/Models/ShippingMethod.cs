using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class ShippingMethod : BaseEntity
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public decimal Fee { get; set; }

        [Required]
        public TimeSpan EstimatedTime { get; set; }

        // Quan hệ ngược nếu cần:
        public ICollection<Order> Orders { get; set; }
    }

}
