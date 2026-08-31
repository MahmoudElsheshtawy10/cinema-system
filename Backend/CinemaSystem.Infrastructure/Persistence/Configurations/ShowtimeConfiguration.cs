#nullable enable
using CinemaSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaSystem.Infrastructure.Persistence.Configurations;

public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new { s.HallId, s.StartTime, s.EndTime });

        builder.Property(s => s.BasePrice).HasColumnType("decimal(18,2)");
    }
}
