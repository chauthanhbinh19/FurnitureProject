using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(VoucherDTO dto);
        Task<(bool Success, string? Message)> UpdateAsync(VoucherDTO dto);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }
}
