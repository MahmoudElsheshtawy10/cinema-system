#nullable enable
using System;

namespace CinemaSystem.Application.Common.Exceptions;

public class SeatConflictException : Exception
{
    public SeatConflictException(string message) : base(message) { }
}
