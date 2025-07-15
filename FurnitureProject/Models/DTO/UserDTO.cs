using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models.DTO
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.UserUsername)]
        public string Username { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.UserEmail)]
        public string Email { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.UserPassword)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        [Display(Name = AppConstants.Display.UserFullname)]
        public string? FullName { get; set; }
        [Display(Name = AppConstants.Display.UserPhoneNumber)]
        public string? PhoneNumber { get; set; }
        [Display(Name = AppConstants.Display.UserRole)]
        public string Role { get; set; } = "user";
        [Display(Name = AppConstants.Display.UserStatus)]
        public string Status { get; set; } = "active";
        public DateTime? CreatedAt { get; set; }
    }
}
