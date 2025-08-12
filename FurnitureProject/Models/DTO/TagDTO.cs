using FurnitureProject.Constants;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class TagDTO
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.TagNameCannotBeEmpty)]
        [Display(Name = AppConstants.Display.TagName)]
        public string Name { get; set; }
        [Display(Name = AppConstants.Display.TagStatus)]
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
