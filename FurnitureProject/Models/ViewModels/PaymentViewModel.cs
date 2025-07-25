using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class PaymentViewModel
    {
        public OrderDTO Order { get; set; }
        public List<ProductDTO> ProductsInCart { get; set; }
        public decimal TotalAmount => ProductsInCart?.Sum(p => 
            (p.DiscountPrice > 0 ? p.DiscountPrice : p.Price) * p.Quantity) ?? 0;
    }

}
