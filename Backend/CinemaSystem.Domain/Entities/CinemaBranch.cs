#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class CinemaBranch : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Hall> _halls = new();
    public IReadOnlyCollection<Hall> Halls => _halls.AsReadOnly();

    private CinemaBranch() { } // For EF Core

    public CinemaBranch(string name, string city, string address, double latitude, double longitude)
    {
        Id = Guid.NewGuid();
        Name = name;
        City = city;
        Address = address;
        Latitude = latitude;
        Longitude = longitude;
        IsActive = true;
    }

    public void AddHall(Hall hall)
    {
        _halls.Add(hall);
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
