using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _voucherRepository.GetAllAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _voucherRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message)> CreateAsync(VoucherDTO dto)
        {
            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit,
                TimeUsed = dto.TimeUsed,
                Status = AppConstants.Status.Active, // Default status
                CreatedAt = DateTime.UtcNow
            };

            voucher.ProductVouchers = dto.SelectedProductIds.Select(productId => new ProductVoucher
            {
                ProductId = productId,
                VoucherId = voucher.Id
            }).ToList();

            try
            {
                await _voucherRepository.AddAsync(voucher);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(VoucherDTO dto)
        {
            var voucher = new Voucher
            {
                Id = Guid.NewGuid(),
                Code = dto.Code,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit,
                TimeUsed = dto.TimeUsed,
                Status = AppConstants.Status.Active, // Default status
                CreatedAt = DateTime.UtcNow
            };

            voucher.ProductVouchers = dto.SelectedProductIds.Select(productId => new ProductVoucher
            {
                ProductId = productId,
                VoucherId = voucher.Id
            }).ToList();

            try
            {
                await _voucherRepository.UpdateAsync(voucher);
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
                await _voucherRepository.DeleteAsync(id);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }
    }
}
