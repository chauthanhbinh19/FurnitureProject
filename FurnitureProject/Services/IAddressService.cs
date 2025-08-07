using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(Guid userId);
        Task<Address?> GetAddressByIdAsync(Guid id);
        Task<Address?> GetDefaultAddressAsync(Guid userId);
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(Guid id);
        Task<IEnumerable<Province>> GetProvincesAsync();
        Task<IEnumerable<District>> GetDistrictsAsync(int provinceCode);
        Task<IEnumerable<Ward>> GetWardsAsync(int districtCode);
    }
}
