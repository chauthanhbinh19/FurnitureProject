namespace FurnitureProject.Models.DTO
{
    public class ProductFilterDTO
    {
        public string? SearchKeyWord { get; set; }
        public Guid? FilterCategoryId { get; set; }
        public Guid? FilterTagId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? FilterByStatus { get; set; }
        public string? SortOrder { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
