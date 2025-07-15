using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class PromotionDTO
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.PromotionTitleCannotBeEmpty)]
        [Display(Name = AppConstants.Display.PromotionName)]
        public string? Title { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.PromotionDescriptionCannotBeEmpty)]
        [Display(Name = AppConstants.Display.PromotionDescription)]
        public string? Description { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.PromotionDiscountPercentCannotBeEmpty)]
        [Display(Name = AppConstants.Display.PromotionDiscountPercent)]
        public decimal DiscountPercent { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.PromotionStartDateCannotBeEmpty)]
        [Display(Name = AppConstants.Display.PromotionStartDate)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = AppConstants.LogMessages.PromotionEndDateCannotBeEmpty)]
        [Display(Name = AppConstants.Display.PromotionEndDate)]
        public DateTime EndDate { get; set; }
        [Display(Name = AppConstants.Display.PromotionStatus)]
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<ProductDTO> Products { get; set; } = new();
        public List<Guid> SelectedProductIds { get; set; } = new();
    }
}
