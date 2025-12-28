using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class CreateSeatDtoValidator : AbstractValidator<CreateSeatDto>
{
    public CreateSeatDtoValidator()
    {
        RuleFor(x => x.AuditoriumId)
            .NotEmpty().WithMessage("Auditorium ID is required");

        RuleFor(x => x.Row)
            .NotEmpty().WithMessage("Row is required")
            .MaximumLength(10).WithMessage("Row must not exceed 10 characters");

        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Seat number must be greater than zero");
    }
}

