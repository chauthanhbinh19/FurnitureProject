namespace FurnitureProject.Models
{
    public class Address : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool? IsDefault { get; set; } = false;

        public User User { get; set; }
    }
}
