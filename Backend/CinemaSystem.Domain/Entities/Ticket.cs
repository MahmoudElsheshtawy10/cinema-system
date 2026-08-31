#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class Ticket : AuditableEntity<Guid>
{
    public Guid BookingSeatId { get; private set; }
    public string TicketCode { get; private set; } = string.Empty;
    public string QRCodePayload { get; private set; } = string.Empty;
    public bool IsCheckedIn { get; private set; }
    public DateTime? CheckedInAtUtc { get; private set; }
    public Guid? CheckedInByStaffId { get; private set; }

    private Ticket() { } // For EF Core

    public Ticket(Guid bookingSeatId, string ticketCode, string qrCodePayload)
    {
        Id = Guid.NewGuid();
        BookingSeatId = bookingSeatId;
        TicketCode = ticketCode;
        QRCodePayload = qrCodePayload;
        IsCheckedIn = false;
    }

    public void ValidateAndCheckIn(Guid staffUserId)
    {
        if (IsCheckedIn)
            throw new InvalidOperationException("Ticket is already checked in.");

        IsCheckedIn = true;
        CheckedInAtUtc = DateTime.UtcNow;
        CheckedInByStaffId = staffUserId;
    }
}
