using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Domain.Exceptions;
using Cinema_MGMT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Payments.Commands;

public class CreatePaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public CreatePaymentDto Dto { get; set; } = null!;
    public Guid UserId { get; set; }
}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePaymentCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Validate booking exists and belongs to user using domain method
            var booking = await _unitOfWork.Bookings.GetByIdAsync(request.Dto.BookingId, cancellationToken);
            if (booking == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<PaymentDto>.ErrorResponse("Booking not found");
            }

            if (!booking.BelongsToUser(request.UserId))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<PaymentDto>.ErrorResponse("Unauthorized access to booking");
            }

            // Check if booking can be paid using domain method
            if (!booking.CanBePaid())
            {
                // Check if expired and mark it
                if (booking.IsExpired())
                {
                    booking.MarkAsExpired();
                    await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<PaymentDto>.ErrorResponse($"Booking cannot be paid. Status: {booking.Status}");
            }

            // Check if payment already exists
            var existingPayment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                p => p.BookingId == request.Dto.BookingId,
                cancellationToken);

            if (existingPayment != null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<PaymentDto>.ErrorResponse("Payment already exists for this booking");
            }

            // Create payment using domain constructor
            var payment = new Payment(
                request.Dto.BookingId,
                booking.TotalAmount,
                request.Dto.Method,
                request.Dto.TransactionId,
                request.Dto.Notes);

            // Mark payment as paid (in real scenario, this would be validated by payment gateway)
            payment.MarkAsPaid(request.Dto.TransactionId);

            await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

            // Confirm booking using domain method
            booking.Confirm();
            await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var paymentDto = new PaymentDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                PaymentDate = payment.PaymentDate,
                TransactionId = payment.TransactionId,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };

            _logger.LogInformation("Payment created successfully. PaymentId: {PaymentId}, BookingId: {BookingId}", payment.Id, booking.Id);

            return ApiResponse<PaymentDto>.SuccessResponse(paymentDto, "Payment completed successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when creating payment");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResponse<PaymentDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResponse<PaymentDto>.ErrorResponse("An error occurred while processing the payment");
        }
    }
}

