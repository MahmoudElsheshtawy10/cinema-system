#nullable enable
using System;
using CinemaSystem.Domain.Common;

namespace CinemaSystem.Domain.Entities;

public class Review : AuditableEntity<Guid>
{
    public Guid MovieId { get; private set; }
    public Guid UserId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private Review() { } // For EF Core

    public Review(Guid movieId, Guid userId, int rating, string? comment)
    {
        Id = Guid.NewGuid();
        MovieId = movieId;
        UserId = userId;
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        Rating = rating;
        Comment = comment;
    }
}
