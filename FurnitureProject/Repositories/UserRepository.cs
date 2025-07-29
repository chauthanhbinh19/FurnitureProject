using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task AddAsync(User user)
        {
            try
            {
                user.DateOfBirth = user.DateOfBirth.HasValue
                    ? DateTime.SpecifyKind(user.DateOfBirth.Value, DateTimeKind.Utc)
                    : null;

                await _context.Users.AddAsync(user);
                var affected = await _context.SaveChangesAsync();

                if (affected == 0)
                    Console.WriteLine("No row was inserted.");
                else
                    Console.WriteLine("OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert failed: " + ex.Message);
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            user.DateOfBirth = user.DateOfBirth.HasValue
                ? DateTime.SpecifyKind(user.DateOfBirth.Value, DateTimeKind.Utc)
                : null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

}
