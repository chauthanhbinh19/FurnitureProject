using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Promotion : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        public decimal DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "active";

        public ICollection<ProductPromotion> ProductPromotions { get; set; }
    }
}
