using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models.DTO
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.VoucherCode)]
        public string Code { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.VoucherDiscountPercent)]
        public int DiscountPercent { get; set; }
        [Display(Name = AppConstants.Display.VoucherDiscountAmount)]
        public decimal DiscountAmount { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.VoucherExpiryDate)]
        public DateTime ExpiryDate { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.VoucherUsageLimit)]
        public int UsageLimit { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.VoucherTimeUsed)]
        public int TimeUsed { get; set; }
        [Display(Name = AppConstants.Display.VoucherIsValid)]
        public bool IsValid { get; set; }
        [Display(Name = AppConstants.Display.VoucherStatus)]
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
