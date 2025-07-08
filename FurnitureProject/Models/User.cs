using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "user";
        public string Status { get; set; } = "active";
    }
}
