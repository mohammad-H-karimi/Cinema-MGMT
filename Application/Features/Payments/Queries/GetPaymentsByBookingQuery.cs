using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Payments.Queries;

public class GetPaymentsByBookingQuery : IRequest<ApiResponse<List<PaymentDto>>>
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
}

public class GetPaymentsByBookingQueryHandler : IRequestHandler<GetPaymentsByBookingQuery, ApiResponse<List<PaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentsByBookingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<PaymentDto>>> Handle(GetPaymentsByBookingQuery request, CancellationToken cancellationToken)
    {
        // Verify booking belongs to user
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking == null)
        {
            return ApiResponse<List<PaymentDto>>.ErrorResponse("Booking not found");
        }

        if (!booking.BelongsToUser(request.UserId))
        {
            return ApiResponse<List<PaymentDto>>.ErrorResponse("Unauthorized access to booking");
        }

        var payments = await _unitOfWork.Payments.FindAsync(
            p => p.BookingId == request.BookingId,
            cancellationToken);

        var paymentDtos = payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            BookingId = p.BookingId,
            Amount = p.Amount,
            Method = p.Method,
            Status = p.Status,
            PaymentDate = p.PaymentDate,
            TransactionId = p.TransactionId,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        }).ToList();

        return ApiResponse<List<PaymentDto>>.SuccessResponse(paymentDtos);
    }
}

