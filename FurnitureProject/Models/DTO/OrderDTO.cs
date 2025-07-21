using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Display(Name = AppConstants.Display.OrderReceiverName)]
        public string? ReceiverName { get; set; }
        [Display(Name = AppConstants.Display.OrderReceiverEmail)]
        public string? ReceiverEmail { get; set; }
        [Display(Name = AppConstants.Display.OrderReceiverPhone)]
        public string? ReceiverPhone { get; set; }
        [Display(Name = AppConstants.Display.OrderShippingAddress)]
        public string? ShippingAddress { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        public string Status { get; set; } = "Pending";
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
