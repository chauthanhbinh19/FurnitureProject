using FurnitureProject.Data;

namespace FurnitureProject.Models
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        FullName = "Nguyễn Văn A",
                        Email = "a@example.com",
                        Username = "thanhbinh19",
                        Password = "123456", // demo only
                        Role = "admin",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Trần Thị B",
                        Email = "b@example.com",
                        Username = "thanhhiep19",
                        Password = "123456",
                        Role = "user",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Bàn", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Ghế", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Tủ", CreatedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var categories = context.Categories.ToList();
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Bàn Làm Việc",
                        Description = "Bàn gỗ công nghiệp",
                        Price = 1500000,
                        CategoryId = categories.First(c => c.Name == "Bàn").Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Product
                    {
                        Name = "Ghế Sofa",
                        Description = "Ghế bọc nỉ cao cấp",
                        Price = 2500000,
                        CategoryId = categories.First(c => c.Name == "Ghế").Id,
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.ProductImages.Any())
            {
                var products = context.Products.ToList();
                context.ProductImages.AddRange(
                    new ProductImage
                    {
                        ProductId = products[0].Id,
                        ImageUrl = "/images/ban.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductImage
                    {
                        ProductId = products[1].Id,
                        ImageUrl = "/images/ghe.jpg",
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            if (!context.Promotions.Any())
            {
                context.Promotions.AddRange(
                    new Promotion
                    {
                        Title = "Khuyến mãi hè",
                        Description = "Giảm giá 10%",
                        DiscountPercent = 10,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(10),
                        CreatedAt = DateTime.UtcNow
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.ProductPromotions.Any())
            {
                var product = context.Products.First();
                var promo = context.Promotions.First();
                context.ProductPromotions.Add(new ProductPromotion
                {
                    ProductId = product.Id,
                    PromotionId = promo.Id
                });
            }

            if (!context.Orders.Any())
            {
                var user = context.Users.First();
                var product = context.Products.First();
                context.Orders.Add(new Order
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    TotalAmount = product.Price,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = new[]
                    {
                        new OrderItem
                        {
                            ProductId = product.Id,
                            Quantity = 1,
                            UnitPrice = product.Price,
                            CreatedAt = DateTime.UtcNow
                        }
                    }
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
