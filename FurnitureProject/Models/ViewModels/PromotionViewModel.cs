using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class PromotionViewModel
    {
        public List<PromotionDTO> Promotions { get; set; } = new();
        public PromotionFilterDTO Filter { get; set; } = new();
    }
}
