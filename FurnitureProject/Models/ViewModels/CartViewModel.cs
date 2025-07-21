using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class CartViewModel
    {
        public Cart Cart { get; set; } = new Cart();
        public List<ProductDTO> ProductsInCart { get; set; } = new List<ProductDTO>();
    }
}
