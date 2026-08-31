#nullable enable
using FluentValidation;

namespace CinemaSystem.Application.Features.Bookings.Commands.LockSeats;

public class LockSeatsCommandValidator : AbstractValidator<LockSeatsCommand>
{
    public LockSeatsCommandValidator()
    {
        RuleFor(x => x.ShowtimeId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SeatIds)
            .NotEmpty().WithMessage("At least one seat must be selected.")
            .Must(s => s != null && s.Count <= 10).WithMessage("Cannot book more than 10 seats at once.");
    }
}
