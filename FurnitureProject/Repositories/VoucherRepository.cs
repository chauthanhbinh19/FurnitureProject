using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AppDbContext _context;

        public VoucherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllAsync()
        {
            return await _context.Vouchers
                .Where(v => !v.IsDeleted)
                .Include(v => v.ProductVouchers)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.Vouchers
                .Where(v => !v.IsDeleted)
                .Include(v => v.ProductVouchers)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task AddAsync(Voucher voucher)
        {
            voucher.ExpiryDate = DateTime.SpecifyKind(voucher.ExpiryDate, DateTimeKind.Utc);
            await _context.Vouchers.AddAsync(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Voucher voucher)
        {
            voucher.ExpiryDate = DateTime.SpecifyKind(voucher.ExpiryDate, DateTimeKind.Utc);
            _context.Vouchers.Update(voucher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var discountCode = await _context.Vouchers.FindAsync(id);
            if (discountCode != null)
            {
                _context.Vouchers.Remove(discountCode);
                await _context.SaveChangesAsync();
            }
        }
    }
}
