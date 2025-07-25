namespace FurnitureProject.Models.DTO
{
    public class VoucherFilterDTO
    {
        public string? SearchKeyWord { get; set; }
        public string? FilterByStatus { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
    }
}
