using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models
{
    public class DiscountCode : BaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]  
        
        public string Code { get; set; }
        [Range(0,100)]
        public int DiscountPercent {get; set;}
        public decimal DiscountAmount { get; set;}
        public DateTime ExpiryDate { get; set;}
        public int UsageLimit { get; set;}
        public int TimeUsed { get; set;}
        [NotMapped]
        public bool IsValid => DateTime.UtcNow < ExpiryDate && TimeUsed < UsageLimit;
        public ICollection<ProductDiscountCode> ProductDiscountCodes { get; set;}
    }
}
