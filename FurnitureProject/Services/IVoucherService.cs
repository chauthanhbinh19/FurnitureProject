using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IVoucherService
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(Voucher discountCode);
        Task<(bool Success, string? Message)> UpdateAsync(Voucher discountCode);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }
}
