using Cinema_MGMT.Application.DTOs;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class UpdateMovieDtoValidator : AbstractValidator<UpdateMovieDto>
{
    public UpdateMovieDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Movie ID is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than zero");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required")
            .MaximumLength(100).WithMessage("Genre must not exceed 100 characters");

        RuleFor(x => x.Director)
            .NotEmpty().WithMessage("Director is required")
            .MaximumLength(200).WithMessage("Director must not exceed 200 characters");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty().WithMessage("Release date is required");

        RuleFor(x => x.PosterUrl)
            .MaximumLength(500).WithMessage("Poster URL must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.PosterUrl));

        RuleFor(x => x.TicketPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Ticket price must be greater than or equal to zero");
    }
}

