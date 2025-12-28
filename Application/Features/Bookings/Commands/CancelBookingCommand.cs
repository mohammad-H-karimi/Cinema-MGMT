using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Bookings.Commands;

public class CancelBookingCommand : IRequest<ApiResponse<BookingDto>>
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelBookingCommandHandler> _logger;

    public CancelBookingCommandHandler(IUnitOfWork unitOfWork, ILogger<CancelBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<BookingDto>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId, cancellationToken);
            
            if (booking == null)
            {
                return ApiResponse<BookingDto>.ErrorResponse("Booking not found");
            }

            if (!booking.BelongsToUser(request.UserId))
            {
                return ApiResponse<BookingDto>.ErrorResponse("Unauthorized access to booking");
            }

            booking.Cancel();
            await _unitOfWork.Bookings.UpdateAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                ScreeningId = booking.ScreeningId,
                UserId = booking.UserId,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                BookingDate = booking.BookingDate,
                ExpiresAt = booking.ExpiresAt,
                CreatedAt = booking.CreatedAt
            };

            _logger.LogInformation("Booking cancelled successfully. BookingId: {BookingId}, UserId: {UserId}", 
                booking.Id, request.UserId);

            return ApiResponse<BookingDto>.SuccessResponse(bookingDto, "Booking cancelled successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when cancelling booking");
            return ApiResponse<BookingDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking");
            return ApiResponse<BookingDto>.ErrorResponse("An error occurred while cancelling the booking");
        }
    }
}

