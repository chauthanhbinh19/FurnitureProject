using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Order>> GetAllByUserIdAsync(Guid userId)
        {
            return await orderRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Order> GetByIdAsync(Guid id)
        {
            return await orderRepository.GetByIdAsync(id);
        }
        public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
        {
            var allOrders = await GetAllAsync();

            var filteredOrders = allOrders
                .Where(o => o.Status == "completed" && o.OrderDate >= from && o.OrderDate <= to)
                .ToList();

            var totalRevenue = filteredOrders.Sum(o => o.TotalAmount);
            var totalTransactions = filteredOrders.Count;
            return totalRevenue;
        }

        public async Task<decimal> GetTotalTransactionsAsync(DateTime from, DateTime to)
        {
            var allOrders = await GetAllAsync();

            var filteredOrders = allOrders
                .Where(o => o.Status == "completed" && o.OrderDate >= from && o.OrderDate <= to)
                .ToList();

            var totalTransactions = filteredOrders.Count;
            return totalTransactions;
        }
        
        public async Task<List<(Guid UserId, string FullName, int OrderCount)>> GetTopCustomersAsync(DateTime from, DateTime to)
        {
            var orders = await GetAllAsync();

            var filteredOrders = orders
                .Where(o => o.OrderDate >= from && o.OrderDate <= to && o.Status == "completed")
                .ToList();

            var topCustomers = filteredOrders
                .GroupBy(o => new { o.UserId, o.User.FullName })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.FullName,
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(6)
                .ToList();

            return topCustomers
                .Select(x => (x.UserId, x.FullName, x.OrderCount))
                .ToList();
        }
        
        public async Task<List<ProductDTO>> GetTopProductsAsync(DateTime from, DateTime to)
        {
            var orders = await GetAllAsync();

            var filteredOrders = orders
                .Where(o => o.OrderDate >= from && o.OrderDate <= to && o.Status == "completed")
                .ToList();

            var topProducts = filteredOrders
                .SelectMany(o => o.OrderItems)
                .GroupBy(od => od.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(3)
                .Select(x => new ProductDTO
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Quantity = x.TotalQuantity,
                    ImageUrls = x.Product.ProductImages
                        .Select(img => img.ImageUrl)
                        .ToList()
                })
                .ToList();

            return topProducts;
        }

        public async Task<Dictionary<string, int>> GetOrderCountByStatusAsync(DateTime from, DateTime to)
        {
            var orders = await GetAllAsync();

            var filteredOrders = orders
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionary(g => g.Status, g => g.Count);

            return filteredOrders;
        }

        public async Task<Dictionary<DateTime, decimal>> GetRevenueByDateAsync(DateTime from, DateTime to)
        {
            var orders = await GetAllAsync();

            var revenueByDate = orders
                .Where(o => o.OrderDate.Date >= from.Date && o.OrderDate.Date <= to.Date)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .ToDictionary(g => g.Date, g => g.Revenue);

            return revenueByDate;
        }


        public async Task<(bool Success, string? Message)> PaymentAsync(OrderDTO dto)
        {
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    ReceiverName = dto.ReceiverName,
                    ReceiverEmail = dto.ReceiverEmail,
                    ReceiverPhone = dto.ReceiverPhone,
                    AddressId = dto.AddressId,
                    //ShippingMethodId = dto.ShippingMethodId,
                    PaymentMethod = dto.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = dto.Status,
                    IsPaid = dto.IsPaid,
                    TotalAmount = dto.TotalAmount
                };

                order.OrderItems = dto.Products.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity,
                    UnitPrice = p.DiscountPrice > 0 ? p.DiscountPrice : p.Price
                }).ToList();

                await orderRepository.AddAsync(order);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> CreateAsync(OrderDTO dto)
        {
            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = dto.UserId,
                    ReceiverName = dto.ReceiverName,
                    ReceiverEmail = dto.ReceiverEmail,
                    ReceiverPhone = dto.ReceiverPhone,
                    AddressId = dto.AddressId,
                    //ShippingMethodId = dto.ShippingMethodId,
                    //ShippingFee = dto.ShippingFee,
                    PaymentMethod = dto.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = dto.Status,
                    IsPaid = dto.IsPaid,
                    TotalAmount = dto.TotalAmount
                };

                order.OrderItems = dto.Products.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity,
                    UnitPrice = p.DiscountPrice > 0 ? p.DiscountPrice : p.Price
                }).ToList();

                await orderRepository.AddAsync(order);
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
                var existingOrder = await orderRepository.GetByIdAsync(dto.Id);

                existingOrder.Id = dto.Id;
                existingOrder.UserId = dto.UserId;
                existingOrder.ReceiverName = dto.ReceiverName;
                existingOrder.ReceiverEmail = dto.ReceiverEmail;
                existingOrder.ReceiverPhone = dto.ReceiverPhone;
                existingOrder.AddressId = dto.AddressId;
                //existingOrder.ShippingMethodId = dto.ShippingMethodId;
                //ShippingFee = dto.ShippingFee;
                existingOrder.PaymentMethod = dto.PaymentMethod;
                existingOrder.OrderDate = dto.OrderDate;
                existingOrder.Status = dto.Status;
                existingOrder.IsPaid = dto.IsPaid;
                existingOrder.UpdatedAt = DateTime.UtcNow;
                existingOrder.TotalAmount = dto.TotalAmount;
                existingOrder.OrderItems.Clear();
                existingOrder.OrderItems = dto.Products.Select(p => new OrderItem
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity,
                    UnitPrice = p.DiscountPrice > 0 ? p.DiscountPrice : p.Price
                }).ToList();

                await orderRepository.UpdateAsync(existingOrder);
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
