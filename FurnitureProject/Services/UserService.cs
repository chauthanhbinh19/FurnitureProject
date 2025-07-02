using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepo.GetAllAsync();

        public async Task<User?> GetByIdAsync(int id) => await _userRepo.GetByIdAsync(id);

        public async Task<User?> GetByUsernameAsync(string username) => await _userRepo.GetByUsernameAsync(username);

        public async Task<User?> GetByEmailAsync(string email) => await _userRepo.GetByEmailAsync(email);

        public async Task CreateAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            await _userRepo.AddAsync(user);
        }

        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepo.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepo.DeleteAsync(user);
            }
        }
    }

}
