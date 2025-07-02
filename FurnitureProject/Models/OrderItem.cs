using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class OrderItem : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // lưu tại thời điểm đặt hàng
    }
}
