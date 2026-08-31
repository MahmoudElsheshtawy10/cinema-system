#nullable enable
using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaSystem.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.HasIndex(b => b.BookingReference).IsUnique();
        builder.Property(b => b.BookingReference).IsRequired().HasMaxLength(50);

        builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(b => b.FinalAmount).HasColumnType("decimal(18,2)");

        builder.HasMany(b => b.BookingSeats)
               .WithOne()
               .HasForeignKey(bs => bs.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.BookingFoodItems)
               .WithOne()
               .HasForeignKey(bfi => bfi.BookingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Payments)
               .WithOne()
               .HasForeignKey(p => p.BookingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
