#nullable enable
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CinemaSystem.Application.Common.Interfaces;
using CinemaSystem.Application.Interfaces;
using CinemaSystem.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CinemaSystem.API.Services;

public class PaymobPaymentService : IPaymentService
{
    private readonly ILogger<PaymobPaymentService> _logger;
    private readonly string _hmacSecret;

    public PaymobPaymentService(ILogger<PaymobPaymentService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _hmacSecret = configuration["Paymob:HmacSecret"] ?? "default_secret";
    }

    public Task<PaymentInitiationResult> InitiatePaymentAsync(PaymentRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Initiating Paymob payment for Booking {BookingId} with amount {Amount}", request.BookingId, request.Amount);
        
        var transactionRef = $"PAYMOB-{Guid.NewGuid()}";
        var checkoutUrl = $"https://accept.paymob.com/api/acceptance/iframes/123456?payment_token=mock_token_for_{request.BookingId}";
        
        return Task.FromResult(new PaymentInitiationResult(transactionRef, checkoutUrl));
    }

    public bool VerifyWebhookSignature(string payload, string receivedSignature)
    {
        if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(receivedSignature))
            return false;

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

        return computedSignature.Equals(receivedSignature, StringComparison.OrdinalIgnoreCase);
    }
}
