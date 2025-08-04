using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;

        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await context.Orders
                .Where(o => !o.IsDeleted)
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(pi => pi.ProductImages)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllByUserIdAsync(Guid userId)
        {
            return await context.Orders
                .Where(o => !o.IsDeleted && o.UserId == userId)
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(pi => pi.ProductImages)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await context.Orders
                .Where(o => !o.IsDeleted)
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(pi => pi.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await context.Orders.FindAsync(id);
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
        }
    }
}
