using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(Guid userId)
        {
            return await _addressRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Address?> GetAddressByIdAsync(Guid id)
        {
            return await _addressRepository.GetByIdAsync(id);
        }

        public async Task<Address?> GetDefaultAddressAsync(Guid userId)
        {
            return await _addressRepository.GetDefaultByUserIdAsync(userId);
        }

        public async Task AddAddressAsync(Address address)
        {
            var existingDefault = await _addressRepository.GetDefaultByUserIdAsync(address.UserId);
            if (existingDefault != null)
            {
                existingDefault.IsDefault = false;
                await _addressRepository.UpdateAsync(existingDefault);
            }

            await _addressRepository.AddAsync(address);
        }

        public async Task UpdateAddressAsync(Address address)
        {
            var existingDefault = await _addressRepository.GetDefaultByUserIdAsync(address.UserId);
            if (existingDefault != null && existingDefault.Id != address.Id)
            {
                existingDefault.IsDefault = false;
                await _addressRepository.UpdateAsync(existingDefault);
            }

            await _addressRepository.UpdateAsync(address);
        }

        public async Task DeleteAddressAsync(Guid id)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address != null)
            {
                await _addressRepository.DeleteAsync(address);
            }
        }
    }
}
