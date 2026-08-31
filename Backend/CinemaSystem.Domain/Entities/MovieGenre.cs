#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class MovieGenre : BaseEntity<Guid>
{
    public Guid MovieId { get; private set; }
    public Guid GenreId { get; private set; }

    private MovieGenre() { } // For EF Core

    public MovieGenre(Guid movieId, Guid genreId)
    {
        Id = Guid.NewGuid();
        MovieId = movieId;
        GenreId = genreId;
    }
}
