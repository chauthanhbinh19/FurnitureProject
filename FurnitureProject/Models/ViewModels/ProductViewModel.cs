using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class ProductViewModel
    {
        public List<ProductDTO> Products { get; set; } = new();
        public ProductFilterDTO Filter { get; set; } = new();
    }
}
