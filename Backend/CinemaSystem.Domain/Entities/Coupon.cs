#nullable enable
using System;
using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;

namespace CinemaSystem.Domain.Entities;

public class Coupon : AuditableEntity<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public DiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }
    public DateTime ExpiryDateUtc { get; private set; }
    public int UsageLimit { get; private set; }
    public int TimesUsed { get; private set; }
    public bool IsActive { get; private set; }

    private Coupon() { } // For EF Core

    public Coupon(string code, DiscountType discountType, decimal discountValue, DateTime expiryDateUtc, int usageLimit)
    {
        Id = Guid.NewGuid();
        Code = code;
        DiscountType = discountType;
        DiscountValue = discountValue;
        ExpiryDateUtc = expiryDateUtc;
        UsageLimit = usageLimit;
        TimesUsed = 0;
        IsActive = true;
    }

    public bool CanBeUsed()
    {
        return IsActive && DateTime.UtcNow <= ExpiryDateUtc && TimesUsed < UsageLimit;
    }

    public void IncrementUsage()
    {
        if (CanBeUsed())
        {
            TimesUsed++;
            if (TimesUsed >= UsageLimit)
            {
                IsActive = false;
            }
        }
        else
        {
            throw new InvalidOperationException("Coupon cannot be used.");
        }
    }

    public void Deactivate() => IsActive = false;
}
