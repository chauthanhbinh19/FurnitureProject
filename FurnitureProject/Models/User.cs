using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models
{
    public class User : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; } = "user";
    }
}
