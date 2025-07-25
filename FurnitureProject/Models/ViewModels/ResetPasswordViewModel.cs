using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = AppConstants.Display.UserPassword)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = AppConstants.Display.UserConfirmPassword)]
        public string ConfirmPassword { get; set; }
    }

}
