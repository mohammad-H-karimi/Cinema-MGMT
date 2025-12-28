using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Seats.Commands;

public class DeleteSeatCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteSeatCommandHandler : IRequestHandler<DeleteSeatCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSeatCommandHandler> _logger;

    public DeleteSeatCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSeatCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteSeatCommand request, CancellationToken cancellationToken)
    {
        var seat = await _unitOfWork.Seats.GetByIdAsync(request.Id, cancellationToken);
        if (seat == null)
        {
            return ApiResponse<bool>.ErrorResponse("Seat not found");
        }

        // Check if seat is being used in active bookings
        var activeBookings = await _unitOfWork.BookingSeats.FindAsync(
            bs => bs.SeatId == request.Id,
            cancellationToken);

        if (activeBookings.Any())
        {
            var bookingIds = activeBookings.Select(bs => bs.BookingId).Distinct().ToList();
            var bookings = await _unitOfWork.Bookings.FindAsync(
                b => bookingIds.Contains(b.Id) && 
                     (b.Status == Domain.Enums.BookingStatus.Pending || b.Status == Domain.Enums.BookingStatus.Confirmed),
                cancellationToken);

            if (bookings.Any())
            {
                return ApiResponse<bool>.ErrorResponse("Cannot delete seat that is part of active bookings");
            }
        }

        // Deactivate instead of delete (soft delete)
        seat.Deactivate();
        await _unitOfWork.Seats.UpdateAsync(seat, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seat deleted (deactivated) successfully. SeatId: {SeatId}", request.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Seat deleted successfully");
    }
}

