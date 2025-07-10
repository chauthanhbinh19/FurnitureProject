namespace FurnitureProject.Models
{
    public class ProductDiscountCode : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid DiscountCodeId { get; set; }
        public DiscountCode DiscountCode { get; set; }
    }
}
