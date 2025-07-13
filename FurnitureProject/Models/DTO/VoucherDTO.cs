using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureProject.Models.DTO
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public int DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimeUsed { get; set; }
        public bool IsValid { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
