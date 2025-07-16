using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(OrderDTO dto);
        Task<(bool Success, string? Message)> UpdateAsync(OrderDTO dto);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }
}
