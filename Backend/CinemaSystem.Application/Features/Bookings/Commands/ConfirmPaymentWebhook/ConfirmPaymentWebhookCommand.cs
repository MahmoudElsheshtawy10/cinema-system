#nullable enable
using System;
using MediatR;

namespace CinemaSystem.Application.Features.Bookings.Commands.ConfirmPaymentWebhook;

public record ConfirmPaymentWebhookCommand(
    string RawPayload,
    string Signature,
    string TransactionReference,
    Guid BookingId,
    decimal AmountPaid
) : IRequest<bool>;
