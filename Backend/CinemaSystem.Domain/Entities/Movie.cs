#nullable enable
using System;
using System.Collections.Generic;
using CinemaSystem.Domain.Common;
using CinemaSystem.Domain.Enums;

namespace CinemaSystem.Domain.Entities;

public class Movie : AuditableEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationMinutes { get; private set; }
    public DateTime ReleaseDate { get; private set; }
    public string Language { get; private set; } = string.Empty;
    public string AgeRating { get; private set; } = string.Empty;
    public string? TrailerUrl { get; private set; }
    public string? PosterUrl { get; private set; }
    public MovieStatus Status { get; private set; }

    private readonly List<MovieGenre> _movieGenres = new();
    public IReadOnlyCollection<MovieGenre> MovieGenres => _movieGenres.AsReadOnly();

    private readonly List<Showtime> _showtimes = new();
    public IReadOnlyCollection<Showtime> Showtimes => _showtimes.AsReadOnly();

    private readonly List<Review> _reviews = new();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    private Movie() { } // For EF Core

    public Movie(string title, string description, int durationMinutes, DateTime releaseDate, string language, string ageRating, MovieStatus status)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DurationMinutes = durationMinutes;
        ReleaseDate = releaseDate;
        Language = language;
        AgeRating = ageRating;
        Status = status;
    }

    public void SetMediaUrls(string trailerUrl, string posterUrl)
    {
        TrailerUrl = trailerUrl;
        PosterUrl = posterUrl;
    }

    public void ChangeStatus(MovieStatus status)
    {
        Status = status;
    }
}
