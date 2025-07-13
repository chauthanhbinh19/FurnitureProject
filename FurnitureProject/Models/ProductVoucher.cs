namespace FurnitureProject.Models
{
    public class ProductVoucher : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid VoucherId { get; set; }
        public Voucher Voucher { get; set; }
    }
}
