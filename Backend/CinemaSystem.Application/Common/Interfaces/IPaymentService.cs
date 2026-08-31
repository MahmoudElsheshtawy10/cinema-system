#nullable enable
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.DTOs;

namespace CinemaSystem.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<PaymentInitiationResult> InitiatePaymentAsync(PaymentRequest request, CancellationToken ct);
    bool VerifyWebhookSignature(string payload, string receivedSignature);
}
