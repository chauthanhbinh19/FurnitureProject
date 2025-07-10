using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class CategoryViewModel
    {
        public List<CategoryDTO> Categories { get; set; } = new();
        public CategoryFilterDTO Filter { get; set; } = new();
    }
}
