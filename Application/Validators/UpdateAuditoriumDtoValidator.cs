using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class UpdateAuditoriumDtoValidator : AbstractValidator<UpdateAuditoriumDto>
{
    public UpdateAuditoriumDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Auditorium ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero");
    }
}

