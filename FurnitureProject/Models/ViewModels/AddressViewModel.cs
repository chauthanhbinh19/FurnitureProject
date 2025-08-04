using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class AddressViewModel
    {
        public List<AddressDTO> Addresses { get; set; } = new();
        public AddressFilterDTO Filter { get; set; } = new();
    }
}
