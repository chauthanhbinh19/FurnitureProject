using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FurnitureProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IAddressRepository _addressRepo;

        public UserService(IUserRepository userRepo, IAddressRepository addressRepo)
        {
            _userRepo = userRepo;
            _addressRepo = addressRepo;
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _userRepo.GetAllAsync();

        public async Task<User?> GetByIdAsync(Guid id) => await _userRepo.GetByIdAsync(id);

        public async Task<User?> GetByUsernameAsync(string username) => await _userRepo.GetByUsernameAsync(username);

        public async Task<User?> GetByEmailAsync(string email) => await _userRepo.GetByEmailAsync(email);

        public async Task<(bool Success, string? Message)> CreateAsync(UserDTO userDTO)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = userDTO.FullName,
                    Username = userDTO.Username,
                    Email = userDTO.Email,
                    PhoneNumber = userDTO.PhoneNumber,
                    Password = userDTO.Password, // Ensure password is hashed in the service
                    DateOfBirth = userDTO.DateOfBirth,
                    Gender = userDTO.Gender,
                    EmailConfirmed = userDTO.EmailConfirmed,
                    PhoneNumberConfirmed = userDTO.PhoneNumberConfirmed,
                    AvatarUrl = userDTO.AvatarUrl,
                    Role = userDTO.Role,
                    Status = userDTO.Status,
                    CreatedAt = userDTO.CreatedAt
                };

                user.CreatedAt = DateTime.UtcNow;
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, user.Password);
                await _userRepo.AddAsync(user);

                bool hasDefault = userDTO.Addresses.Any(a => a.IsDefault == true);

                for(int i = 0; i < userDTO.Addresses.Count; i++)
                {
                    var addressDTO = userDTO.Addresses[i];

                    var address = new Address
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Street = addressDTO.Street,
                        Ward = addressDTO.Ward,
                        District = addressDTO.District,
                        City = addressDTO.City,
                        Country = addressDTO.Country,
                        PostalCode = addressDTO.PostalCode,
                        IsDefault = addressDTO.IsDefault ?? false
                    };

                    if (!hasDefault && i == 0)
                    {
                        address.IsDefault = true;
                    }

                    await _addressRepo.AddAsync(address);
                }

                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(UserDTO userDTO)
        {
            try
            {
                var existingUser = await _userRepo.GetByIdAsync(userDTO.Id);
                if (existingUser == null)
                    return (false, "User not found");

                existingUser.FullName = userDTO.FullName;
                existingUser.Username = userDTO.Username;
                existingUser.Email = userDTO.Email;
                existingUser.PhoneNumber = userDTO.PhoneNumber;
                existingUser.DateOfBirth = userDTO.DateOfBirth;
                existingUser.Gender = userDTO.Gender;
                existingUser.EmailConfirmed = userDTO.EmailConfirmed;
                existingUser.PhoneNumberConfirmed = userDTO.PhoneNumberConfirmed;
                existingUser.AvatarUrl = userDTO.AvatarUrl;
                existingUser.Role = userDTO.Role;
                existingUser.Status = userDTO.Status;
                existingUser.UpdatedAt = DateTime.UtcNow;


                if (userDTO.Password != existingUser.Password)
                {
                    var hasher = new PasswordHasher<User>();
                    userDTO.Password = hasher.HashPassword(existingUser, existingUser.Password);
                }

                await _userRepo.UpdateAsync(existingUser);

                //Handle addresses
                var existingAddresses = await _addressRepo.GetAllByUserIdAsync(existingUser.Id);

                //Check for addresses to set default
                bool hasDefault = userDTO.Addresses.Any(a => a.IsDefault == true);
                for (int i = 0; i < userDTO.Addresses.Count; i++)
                {
                    if (!hasDefault && i == 0)
                        userDTO.Addresses[i].IsDefault = true;
                }

                foreach (var addressDTO in userDTO.Addresses)
                {
                    var existing = existingAddresses.FirstOrDefault(a => a.Id == addressDTO.Id);

                    if (existing != null)
                    {
                        // Update existing address
                        existing.Street = addressDTO.Street;
                        existing.Ward = addressDTO.Ward;
                        existing.District = addressDTO.District;
                        existing.City = addressDTO.City;
                        existing.Country = addressDTO.Country;
                        existing.PostalCode = addressDTO.PostalCode;
                        existing.IsDefault = addressDTO.IsDefault ?? false;

                        await _addressRepo.UpdateAsync(existing);
                    }
                    else
                    {
                        // Insert new address
                        var newAddress = new Address
                        {
                            Id = Guid.NewGuid(),
                            UserId = existingUser.Id,
                            Street = addressDTO.Street,
                            Ward = addressDTO.Ward,
                            District = addressDTO.District,
                            City = addressDTO.City,
                            Country = addressDTO.Country,
                            PostalCode = addressDTO.PostalCode,
                            IsDefault = addressDTO.IsDefault ?? false
                        };
                        await _addressRepo.AddAsync(newAddress);
                    }
                }

                //Delete addresses that are not in the DTO
                var dtoIds = userDTO.Addresses.Where(a => a.Id != Guid.Empty).Select(a => a.Id).ToList();
                var addressesToDelete = existingAddresses.Where(e => !dtoIds.Contains(e.Id)).ToList();

                foreach (var address in addressesToDelete)
                {
                    await _addressRepo.DeleteAsync(address);
                }

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
