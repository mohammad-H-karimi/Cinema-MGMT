using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Seats.Commands;

public class UpdateSeatCommand : IRequest<ApiResponse<SeatDto>>
{
    public UpdateSeatDto Dto { get; set; } = null!;
}

public class UpdateSeatCommandHandler : IRequestHandler<UpdateSeatCommand, ApiResponse<SeatDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSeatCommandHandler> _logger;

    public UpdateSeatCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSeatCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<SeatDto>> Handle(UpdateSeatCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (seat == null)
            {
                return ApiResponse<SeatDto>.ErrorResponse("Seat not found");
            }

            // Check if seat is being used in active bookings
            var activeBookings = await _unitOfWork.BookingSeats.FindAsync(
                bs => bs.SeatId == request.Dto.Id,
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
                    return ApiResponse<SeatDto>.ErrorResponse("Cannot update seat that is part of active bookings");
                }
            }

            // Note: Seat entity doesn't have an Update method, so we can only update IsActive
            // For a full update, you'd need to add an Update method to the domain entity
            if (seat.IsActive != request.Dto.IsActive)
            {
                if (request.Dto.IsActive)
                    seat.Activate();
                else
                    seat.Deactivate();
            }

            await _unitOfWork.Seats.UpdateAsync(seat, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var seatDto = new SeatDto
            {
                Id = seat.Id,
                AuditoriumId = seat.AuditoriumId,
                Row = seat.Row,
                Number = seat.Number,
                IsActive = seat.IsActive,
                CreatedAt = seat.CreatedAt
            };

            _logger.LogInformation("Seat updated successfully. SeatId: {SeatId}", seat.Id);

            return ApiResponse<SeatDto>.SuccessResponse(seatDto, "Seat updated successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when updating seat");
            return ApiResponse<SeatDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating seat");
            return ApiResponse<SeatDto>.ErrorResponse("An error occurred while updating the seat");
        }
    }
}

