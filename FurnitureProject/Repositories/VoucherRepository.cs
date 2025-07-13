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
            return await _context.DiscountCodes
                .Include(dc => dc.ProductDiscountCodes)
                .ToListAsync();
        }

        public async Task<Voucher?> GetByIdAsync(Guid id)
        {
            return await _context.DiscountCodes
                .Include(dc => dc.ProductDiscountCodes)
                .FirstOrDefaultAsync(dc => dc.Id == id);
        }

        public async Task AddAsync(Voucher discountCode)
        {
            await _context.DiscountCodes.AddAsync(discountCode);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Voucher discountCode)
        {
            _context.DiscountCodes.Update(discountCode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode != null)
            {
                _context.DiscountCodes.Remove(discountCode);
                await _context.SaveChangesAsync();
            }
        }
    }
}
