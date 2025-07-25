using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class UserViewModel
    {
        public List<UserDTO> Users { get; set; } = new();
        public UserFilterDTO Filter { get; set; } = new();
    }
}
