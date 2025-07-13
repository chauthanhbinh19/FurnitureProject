using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<Voucher>> GetAllAsync();
        Task<Voucher?> GetByIdAsync(Guid id);
        Task AddAsync(Voucher discountCode);
        Task UpdateAsync(Voucher discountCode);
        Task DeleteAsync(Guid id);
    }
}
