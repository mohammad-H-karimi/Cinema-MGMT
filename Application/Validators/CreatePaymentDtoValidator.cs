using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Domain.Enums;
using FluentValidation;

namespace Cinema_MGMT.Application.Validators;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required");

        RuleFor(x => x.Method)
            .IsInEnum().WithMessage("Invalid payment method");

        RuleFor(x => x.TransactionId)
            .MaximumLength(200).WithMessage("Transaction ID must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.TransactionId));

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

