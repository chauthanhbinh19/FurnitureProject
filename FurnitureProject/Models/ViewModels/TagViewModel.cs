using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class TagViewModel
    {
        public List<TagDTO> Tags { get; set; } = new();
        public TagFilterDTO Filter { get; set; } = new();
    }
}
