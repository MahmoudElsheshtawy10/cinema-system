#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class Genre : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;

    private readonly List<MovieGenre> _movieGenres = new();
    public IReadOnlyCollection<MovieGenre> MovieGenres => _movieGenres.AsReadOnly();

    private Genre() { } // For EF Core

    public Genre(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
