using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Repositories;
using Microsoft.AspNetCore.Identity;

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

        public async Task<User?> GetByIdAsync(Guid id) => await _userRepo.GetByIdAsync(id);

        public async Task<User?> GetByUsernameAsync(string username) => await _userRepo.GetByUsernameAsync(username);

        public async Task<User?> GetByEmailAsync(string email) => await _userRepo.GetByEmailAsync(email);

        public async Task<(bool Success, string? Message)> CreateAsync(User user)
        {
            try
            {
                user.CreatedAt = DateTime.UtcNow;
                await _userRepo.AddAsync(user);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(User user)
        {
            try
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepo.UpdateAsync(user);
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
                var user = await _userRepo.GetByIdAsync(id);
                if (user != null)
                {
                    await _userRepo.DeleteAsync(user);
                }
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> SignInAsync(User user)
        {
            var hasher = new PasswordHasher<User>();
            var userFromDB = await _userRepo.GetByUsernameAsync(user.Username);
            if (userFromDB != null)
            {
                var result = hasher.VerifyHashedPassword(user, userFromDB.Password, user.Password);
                if(result == PasswordVerificationResult.Success)
                {
                    //HttpContext.Session.SetString("UserID");
                    //HttpContext.Session.SetString("UserRole");
                    return (true, null);
                }
            }

            return (false, null);
        }

        public async Task<(bool Success, string? Message)> SignUpAsync(User user)
        {
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password);

            // Set Create time
            user.CreatedAt = DateTime.UtcNow;

            // Set user role
            if (string.IsNullOrEmpty(user.Role))
                user.Role = "admin";

            // Add user
            await _userRepo.AddAsync(user);
            return (true, null);
        }
    }

}
