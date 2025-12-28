using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class UpdateScreeningDtoValidator : AbstractValidator<UpdateScreeningDto>
{
    public UpdateScreeningDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Screening ID is required");

        RuleFor(x => x.MovieId)
            .NotEmpty().WithMessage("Movie ID is required");

        RuleFor(x => x.AuditoriumId)
            .NotEmpty().WithMessage("Auditorium ID is required");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}

