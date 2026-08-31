#nullable enable
using System;

namespace CinemaSystem.Domain.Common;

public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public DateTime CreatedAtUtc { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? LastModifiedAtUtc { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }

    public void SetCreated(string createdBy, DateTime createdAtUtc)
    {
        CreatedBy = createdBy;
        CreatedAtUtc = createdAtUtc;
    }

    public void SetModified(string lastModifiedBy, DateTime lastModifiedAtUtc)
    {
        LastModifiedBy = lastModifiedBy;
        LastModifiedAtUtc = lastModifiedAtUtc;
    }
}
