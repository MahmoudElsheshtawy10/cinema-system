#nullable enable
using System;
using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;

namespace CinemaSystem.Domain.Entities;

public class Payment : AuditableEntity<Guid>
{
    public Guid BookingId { get; private set; }
    public string TransactionReference { get; private set; } = string.Empty;
    public string Provider { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }

    private Payment() { } // For EF Core

    public Payment(Guid bookingId, string transactionReference, string provider, decimal amount)
    {
        Id = Guid.NewGuid();
        BookingId = bookingId;
        TransactionReference = transactionReference;
        Provider = provider;
        Amount = amount;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsCompleted()
    {
        Status = PaymentStatus.Completed;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
    }
}
