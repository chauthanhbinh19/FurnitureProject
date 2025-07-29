using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<(bool Success, string? Message)> CreateAsync(UserDTO userDTO);
        Task<(bool Success, string? Message)> UpdateAsync(UserDTO userDTO);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
        Task<(bool Success, string? Message)> SignInAsync(User user);
        Task<(bool Success, string? Message)> SignUpAsync(User user);
    }
}
