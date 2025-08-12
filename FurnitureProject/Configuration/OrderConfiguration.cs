using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurnitureProject.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.UserId)
                .IsRequired();

            builder.Property(u => u.ReceiverName)
                .IsRequired();

            builder.Property(u => u.ReceiverEmail)
                .IsRequired();

            builder.Property(u => u.ReceiverPhone)
                .IsRequired();

            builder.Property(u => u.AddressId)
                .IsRequired(false);

            builder.Property(u => u.ShippingMethodId)
                .IsRequired(false);

            builder.Property(u => u.ShippingFee)
                .IsRequired();

            builder.Property(u => u.PaymentMethod)
                .IsRequired();

            builder.Property(u => u.OrderDate)
                .IsRequired();

            builder.Property(u => u.Status)
                .IsRequired()
                .HasDefaultValue("active");

            builder.Property(u => u.IsPaid)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.TotalAmount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(u => u.CreatedAt);

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(o => o.Address)
            .WithMany()
            .HasForeignKey(o => o.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
