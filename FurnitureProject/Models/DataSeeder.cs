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
                        Password = "123456",
                        PhoneNumber = "0909123456",
                        Role = "admin",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Trần Thị B",
                        Email = "b@example.com",
                        Username = "thanhhiep19",
                        Password = "123456",
                        PhoneNumber = "0909123457",
                        Role = "user",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        FullName = "Nguyễn Thiên C",
                        Email = "a@example.com",
                        Username = "thanhthien20",
                        Password = "123456",
                        PhoneNumber = "0909123452",
                        Role = "admin",
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
                    new Category { Name = "Tủ", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Giường", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Kệ sách", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Tủ giày", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Sofa", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Bàn học", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Bàn làm việc", CreatedAt = DateTime.UtcNow },
                    new Category { Name = "Bàn ăn", CreatedAt = DateTime.UtcNow }
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
                        ImageUrl = "https://res.cloudinary.com/dewf4msbe/image/upload/v1751963408/No_Image_Available_arromx.jpg",
                        CreatedAt = DateTime.UtcNow
                    },
                    new ProductImage
                    {
                        ProductId = products[1].Id,
                        ImageUrl = "https://res.cloudinary.com/dewf4msbe/image/upload/v1751963408/No_Image_Available_arromx.jpg",
                        CreatedAt = DateTime.UtcNow
                    }
                );
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(
                    new Tag { Name = "Mới", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Giảm giá", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Bán chạy", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Hàng hot", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Cao cấp", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Giá rẻ", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Thiết kế mới", CreatedAt = DateTime.UtcNow },
                    new Tag { Name = "Sản phẩm nổi bật", CreatedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            if (!context.ProductTag.Any())
            {
                var products = context.Products.Take(3).ToList(); // Lấy 3 sản phẩm đầu tiên
                var tags = context.Tags.Take(3).ToList();         // Lấy 3 tag đầu tiên

                var productTags = new List<ProductTag>();

                foreach (var product in products)
                {
                    foreach (var tag in tags)
                    {
                        productTags.Add(new ProductTag
                        {
                            ProductId = product.Id,
                            TagId = tag.Id
                        });
                    }
                }

                context.ProductTag.AddRange(productTags);
                await context.SaveChangesAsync();
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
