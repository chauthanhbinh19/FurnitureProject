using FurnitureProject.Helper;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.ProductName)]
        public string? Name { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.ProductDescription)]
        public string? Description { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.ProductPrice)]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.ProductStock)]
        public int Stock { get; set; }
        [Required]
        [Display(Name = AppConstants.Display.ProductCategory)]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        [Display(Name = AppConstants.Display.ProductStatus)]
        public string Status { get; set; }
        public decimal DiscountPrice { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? PromotionStatus { get; set; }
        public string? VoucherStatus { get; set; }
        public bool IsFavourited { get; set; } = false;
        public int Quantity { get; set; }
        public List<IFormFile>? Files { get; set; }
        public List<string>? ImageUrls { get; set; } = new();
        [Display(Name = AppConstants.Display.ProductTag)]
        public List<Guid>? TagIds { get; set; } = new();
        public List<string>? ExistingImageUrls { get; set; } = new();
    }
}
