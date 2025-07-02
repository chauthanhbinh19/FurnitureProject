using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class Promotion : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public decimal DiscountPercent { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ICollection<ProductPromotion> ProductPromotions { get; set; }
    }
}
