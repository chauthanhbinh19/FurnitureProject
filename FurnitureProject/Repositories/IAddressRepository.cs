using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllByUserIdAsync(Guid userId);
        Task<Address?> GetByIdAsync(Guid id);
        Task<Address?> GetDefaultByUserIdAsync(Guid userId);
        Task AddAsync(Address address);
        Task UpdateAsync(Address address);
        Task DeleteAsync(Address address);
    }
}
