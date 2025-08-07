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
        public Guid? AddressId { get; set; }
        public AddressDTO? Address { get; set; }
        public List<AddressDTO> Addresses { get; set; } = new List<AddressDTO>();
        public Guid? ShippingMethodId { get; set; }
        [Display(Name = AppConstants.Display.OrderShippingFee)]
        public decimal ShippingFee { get; set; }
        [Display(Name = AppConstants.Display.OrderPaymentMethod)]
        public string? PaymentMethod { get; set; }
        [Display(Name = AppConstants.Display.OrderOrderDate)]
        public string? PaymentGateway { get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.OrderStatus)]
        public string Status { get; set; } = "Pending";
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();
        [Display(Name = AppConstants.Display.OrderTotalAmount)]
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
