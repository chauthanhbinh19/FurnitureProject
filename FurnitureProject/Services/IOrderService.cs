using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetAllByUserIdAsync(Guid userId);
        Task<Order> GetByIdAsync(Guid id);
        Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to);
        Task<decimal> GetTotalTransactionsAsync(DateTime from, DateTime to);
        Task<List<(Guid UserId, string FullName, int OrderCount)>> GetTopCustomersAsync(DateTime from, DateTime to);
        Task<List<ProductDTO>> GetTopProductsAsync(DateTime from, DateTime to);
        Task<Dictionary<string, int>> GetOrderCountByStatusAsync(DateTime from, DateTime to);
        Task<Dictionary<DateTime, decimal>> GetRevenueByDateAsync(DateTime from, DateTime to);
        Task<(bool Success, string? Message)> PaymentAsync(OrderDTO dto);
        Task<(bool Success, string? Message)> CreateAsync(OrderDTO dto);
        Task<(bool Success, string? Message)> UpdateAsync(OrderDTO dto);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }
}
