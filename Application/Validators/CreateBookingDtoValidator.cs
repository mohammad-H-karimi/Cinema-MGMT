using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ScreeningId)
            .NotEmpty().WithMessage("Screening ID is required");

        RuleFor(x => x.SeatIds)
            .NotEmpty().WithMessage("At least one seat must be selected")
            .Must(seatIds => seatIds != null && seatIds.Count > 0)
            .WithMessage("At least one seat must be selected")
            .Must(seatIds => seatIds != null && seatIds.Distinct().Count() == seatIds.Count)
            .WithMessage("Duplicate seats are not allowed");
    }
}

