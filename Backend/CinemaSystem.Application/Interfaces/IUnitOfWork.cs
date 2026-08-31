#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CinemaSystem.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
