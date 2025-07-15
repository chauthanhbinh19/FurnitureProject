using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FurnitureProject.Models.DTO
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.CategoryNameCannotBeEmpty)]
        [Display(Name = AppConstants.Display.CategoryName)]
        public string Name { get; set; }
        [Display(Name = AppConstants.Display.CategoryDescription)]
        public string? Description { get; set; }
        [Display(Name = AppConstants.Display.CategoryStatus)]
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
