#nullable enable
using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaSystem.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new { s.HallId, s.RowLabel, s.SeatNumber }).IsUnique();
        
        builder.Property(s => s.RowLabel).IsRequired().HasMaxLength(10);
    }
}
