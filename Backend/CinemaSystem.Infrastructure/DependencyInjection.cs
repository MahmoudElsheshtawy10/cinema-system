#nullable enable
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Infrastructure.Persistence;
using CinemaSystem.Infrastructure.Persistence.Interceptors;
using CinemaSystem.Infrastructure.Persistence.Repositories;
using CinemaSystem.Infrastructure.Services.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace CinemaSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();

        services.AddDbContext<CinemaDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddScoped<IRedisLockService, RedisLockService>();
        
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
