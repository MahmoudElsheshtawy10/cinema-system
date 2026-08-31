#nullable enable
using System;
using CinemaSystem.Domain.Entities;
using CinemaSystem.Domain.Common;
using CinemaSystem.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace CinemaSystem.Infrastructure.Persistence;

public class CinemaDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public CinemaDbContext(
        DbContextOptions<CinemaDbContext> options, 
        AuditableEntityInterceptor auditableEntityInterceptor) 
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<CinemaBranch> CinemaBranches => Set<CinemaBranch>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<MovieGenre> MovieGenres => Set<MovieGenre>();
    public DbSet<Showtime> Showtimes => Set<Showtime>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<BookingFoodItem> BookingFoodItems => Set<BookingFoodItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(CinemaDbContext).Assembly);

        // Apply Global Query Filter for soft deletion
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(AuditableEntity<Guid>).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType).HasQueryFilter(
                    ConvertFilterExpression<AuditableEntity<Guid>>(e => !e.IsDeleted, entityType.ClrType));
            }
        }

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression<TInterface>(
        System.Linq.Expressions.Expression<Func<TInterface, bool>> filterExpression, Type entityType)
    {
        var newParam = System.Linq.Expressions.Expression.Parameter(entityType);
        var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
        return System.Linq.Expressions.Expression.Lambda(newBody, newParam);
    }
}

internal class ReplacingExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
{
    private readonly System.Linq.Expressions.Expression _oldValue;
    private readonly System.Linq.Expressions.Expression _newValue;

    public ReplacingExpressionVisitor(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override System.Linq.Expressions.Expression? Visit(System.Linq.Expressions.Expression? node)
    {
        if (node == _oldValue)
            return _newValue;
        return base.Visit(node);
    }

    public static System.Linq.Expressions.Expression Replace(System.Linq.Expressions.Expression oldValue, System.Linq.Expressions.Expression newValue, System.Linq.Expressions.Expression expression)
    {
        return new ReplacingExpressionVisitor(oldValue, newValue).Visit(expression)!;
    }
}
