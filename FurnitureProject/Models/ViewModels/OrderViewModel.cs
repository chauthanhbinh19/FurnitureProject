using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class OrderViewModel
    {
        public List<OrderDTO> Orders { get; set; } = new();
        public OrderFilterDTO Filter { get; set; } = new();
    }
}
