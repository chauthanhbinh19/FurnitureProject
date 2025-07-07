namespace FurnitureProject.Models.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<IFormFile> Files { get; set; }  // ảnh upload
        public List<string> ImageUrls { get; set; } = new();
    }
}
