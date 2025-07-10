using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class UserViewModel
    {
        public List<User> Users { get; set; } = new();
        public UserFilterDTO Filter { get; set; } = new();
    }
}
