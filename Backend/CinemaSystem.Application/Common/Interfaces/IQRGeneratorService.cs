#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CinemaSystem.Application.Common.Interfaces;

public interface IQRGeneratorService
{
    Task<string> GenerateQRCodeAsync(string payload, CancellationToken ct);
}
