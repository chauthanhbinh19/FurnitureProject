using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class CartViewModel
    {
        public List<ProductDTO> ProductsInCart { get; set; } = new List<ProductDTO>();
    }
}
