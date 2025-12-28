using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class UpdateSeatDtoValidator : AbstractValidator<UpdateSeatDto>
{
    public UpdateSeatDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Seat ID is required");

        RuleFor(x => x.AuditoriumId)
            .NotEmpty().WithMessage("Auditorium ID is required");

        RuleFor(x => x.Row)
            .NotEmpty().WithMessage("Row is required")
            .MaximumLength(10).WithMessage("Row must not exceed 10 characters");

        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Seat number must be greater than zero");
    }
}

