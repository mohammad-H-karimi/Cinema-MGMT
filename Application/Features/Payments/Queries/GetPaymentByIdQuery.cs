using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Payments.Queries;

public class GetPaymentByIdQuery : IRequest<ApiResponse<PaymentDto>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, ApiResponse<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.Id, cancellationToken);

        if (payment == null)
        {
            return ApiResponse<PaymentDto>.ErrorResponse("Payment not found");
        }

        // Verify booking belongs to user
        var booking = await _unitOfWork.Bookings.GetByIdAsync(payment.BookingId, cancellationToken);
        if (booking == null || !booking.BelongsToUser(request.UserId))
        {
            return ApiResponse<PaymentDto>.ErrorResponse("Unauthorized access to payment");
        }

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

        return ApiResponse<PaymentDto>.SuccessResponse(paymentDto);
    }
}

