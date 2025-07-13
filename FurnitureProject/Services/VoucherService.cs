using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _discountCodeRepository;

        public VoucherService(IVoucherRepository discountCodeRepository)
        {
            _discountCodeRepository = discountCodeRepository;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _discountCodeRepository.GetAllAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _discountCodeRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message)> CreateAsync(Voucher discountCode)
        {
            try
            {
                await _discountCodeRepository.AddAsync(discountCode);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(Voucher discountCode)
        {
            try
            {
                await _discountCodeRepository.UpdateAsync(discountCode);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                await _discountCodeRepository.DeleteAsync(id);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }
    }
}
