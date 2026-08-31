#nullable enable
using System;
using MediatR;
using CinemaSystem.Application.DTOs;

namespace CinemaSystem.Application.Features.Staff.Commands.ValidateTicket;

public record ValidateTicketCommand(
    string TicketCode,
    Guid ShowtimeId,
    Guid StaffUserId
) : IRequest<TicketValidationResultDto>;
