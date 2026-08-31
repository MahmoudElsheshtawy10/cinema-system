#nullable enable
using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaSystem.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.TicketCode).IsUnique();
        builder.Property(t => t.TicketCode).IsRequired().HasMaxLength(50);
        builder.Property(t => t.QRCodePayload).IsRequired();

        builder.HasOne<BookingSeat>()
               .WithOne()
               .HasForeignKey<Ticket>(t => t.BookingSeatId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
