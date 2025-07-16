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
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<ProductVoucher> ProductVouchers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostCategoryLink> PostCategoryLinks { get; set; }

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
            modelBuilder.ApplyConfiguration(new VoucherConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVoucherConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new PostCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new PostCategoryLinkConfiguration());

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

            modelBuilder.Entity<ProductVoucher>()
            .HasKey(pt => new { pt.ProductId, pt.VoucherId });

            modelBuilder.Entity<ProductVoucher>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductVouchers)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductVoucher>()
                .HasOne(pt => pt.Voucher)
                .WithMany(t => t.ProductVouchers)
                .HasForeignKey(pt => pt.VoucherId);

            // Order - User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem - Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft-delete global filter (tùy chọn)
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProductImage>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Promotion>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Tag>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProductTag>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Voucher>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProductVoucher>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Post>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PostCategory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<PostCategoryLink>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
