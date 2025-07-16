using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Guid id);
    }
}
