using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await orderRepository.GetAllAsync();
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await orderRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message)> CreateAsync(OrderDTO dto)
        {
            try
            {
                //await orderRepository.AddAsync(order);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(OrderDTO dto)
        {
            try
            {
                //await orderRepository.UpdateAsync(order);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            } 
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                await orderRepository.DeleteAsync(id);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, ex.Message);
            }
        }
    }
}
