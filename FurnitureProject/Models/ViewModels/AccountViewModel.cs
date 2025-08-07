using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class AccountViewModel
    {
        public string CurrentSection { get; set; }

        public UserDTO Profile { get; set; }

        public AddressDTO Address { get; set; }

        public List<AddressDTO> Addresses { get; set; }

        public OrderViewModel OrderViewModel { get; set; } = new();

        public List<VoucherDTO> Vouchers { get; set; }

        public List<ProductDTO> Wishlist { get; set; }
    }
}
