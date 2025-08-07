using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly AppDbContext _context;

        public AddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllByUserIdAsync(Guid userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(Guid id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<Address?> GetDefaultByUserIdAsync(Guid userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault == true && !a.IsDeleted);
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Address address)
        {
            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Province>> GetProvincesAsync()
        {
            return await _context.Provinces
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<District>> GetDistrictsAsync(int provinceCode)
        {
            var districts = _context.Districts
            .Where(d => d.ProvinceCode == provinceCode)
            .OrderBy(d => d.Name)
            .ToList();
            return districts;
        }

        public async Task<IEnumerable<Ward>> GetWardsAsync(int districtCode)
        {
            var wards = _context.Wards
            .Where(w => w.DistrictCode == districtCode)
            .OrderBy(w => w.Name)
            .ToList();
            return wards;
        }
    }
}
