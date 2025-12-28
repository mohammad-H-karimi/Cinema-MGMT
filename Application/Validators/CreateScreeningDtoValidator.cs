using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class CreateScreeningDtoValidator : AbstractValidator<CreateScreeningDto>
{
    public CreateScreeningDtoValidator()
    {
        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage("Movie ID is required");

        RuleFor(x => x.AuditoriumId)
            .NotEmpty().WithMessage("Auditorium ID is required");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .Must(startTime => startTime > DateTime.UtcNow)
            .WithMessage("Start time must be in the future");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}

