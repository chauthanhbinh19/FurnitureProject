using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
