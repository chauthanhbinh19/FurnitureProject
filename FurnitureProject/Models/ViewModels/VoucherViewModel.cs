using FurnitureProject.Models.DTO;

namespace FurnitureProject.Models.ViewModels
{
    public class VoucherViewModel
    {
        public List<VoucherDTO> Vouchers { get; set; } = new();
        public VoucherFilterDTO Filter { get; set; } = new();
    }
}
