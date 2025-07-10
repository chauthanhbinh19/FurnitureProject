using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTag { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ProductPromotion> ProductPromotions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
            modelBuilder.ApplyConfiguration(new PromotionConfiguration());
            modelBuilder.ApplyConfiguration(new ProductPromotionConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ProductTagConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountCodeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDiscountCodeConfiguration());

            // Cấu hình khóa chính kết hợp cho bảng nối nhiều-nhiều
            modelBuilder.Entity<ProductPromotion>()
                .HasKey(pp => new { pp.ProductId, pp.PromotionId });

            modelBuilder.Entity<ProductPromotion>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductPromotions)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductPromotion>()
                .HasOne(pt => pt.Promotion)
                .WithMany(t => t.ProductPromotions)
                .HasForeignKey(pt => pt.PromotionId);

            modelBuilder.Entity<ProductTag>()
            .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);

            modelBuilder.Entity<ProductDiscountCode>()
            .HasKey(pt => new { pt.ProductId, pt.DiscountCodeId });

            modelBuilder.Entity<ProductDiscountCode>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductDiscountCodes)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductDiscountCode>()
                .HasOne(pt => pt.DiscountCode)
                .WithMany(t => t.ProductDiscountCodes)
                .HasForeignKey(pt => pt.DiscountCodeId);

            // Soft-delete global filter (tùy chọn)
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProductImage>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Promotion>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
