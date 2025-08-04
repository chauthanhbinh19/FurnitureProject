namespace FurnitureProject.Models.DTO
{
    public class AddressFilterDTO
    {
        public Guid UserId { get; set; }
        public string? SearchKeyWord { get; set; }
        public string? FilterByStatus { get; set; }
        public string? SortColumn { get; set; }
        public string? SortDirection { get; set; }
    }
}
