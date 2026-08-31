#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Interfaces;

namespace CinemaSystem.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly CinemaDbContext _dbContext;

    public UnitOfWork(CinemaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
