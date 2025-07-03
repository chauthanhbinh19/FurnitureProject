using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.ViewModels
{
    public class EmailVerificationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mã xác nhận")]
        public string Code { get; set; }
    }

}
