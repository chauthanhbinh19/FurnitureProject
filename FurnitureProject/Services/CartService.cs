using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Repositories;
using FurnitureProject.Services.Session;
using System.Security.Claims;

namespace FurnitureProject.Services
{
    public class CartService : ICartService
    {
        private const string SessionCartKey = "Cart";
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<(bool Success, string? Message)> CreateCartAsync(Guid userId, Guid productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return (false, "Product not found");

            try
            {
                var existingCart = await _cartRepository.GetCartByUserIdAsync(userId);

                if (existingCart != null)
                {
                    var existingCartItem = existingCart.CartItems
                        .FirstOrDefault(ci => ci.ProductId == productId);

                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += quantity;
                        await _cartItemRepository.UpdateAsync(existingCartItem); // cập nhật số lượng
                    }
                    else
                    {
                        var newCartItem = new CartItem
                        {
                            CartId = existingCart.Id,
                            ProductId = productId,
                            Quantity = quantity,
                            UnitPrice = product.Price
                        };
                        await _cartItemRepository.CreateAsync(newCartItem); // thêm sản phẩm mới
                    }
                }
                else
                {
                    var newCart = new Cart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    }
                }
                    };

                    await _cartRepository.CreateAsync(newCart);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<bool> RemoveItemAsync(Guid cartItemId)
        {
            return await _cartRepository.RemoveItemAsync(cartItemId);
        }
    }
}
