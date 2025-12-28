using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Domain.Exceptions;
using Cinema_MGMT.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Bookings.Commands;

public class CreateBookingCommand : IRequest<ApiResponse<BookingDto>>
{
    public CreateBookingDto Dto { get; set; } = null!;
    public Guid UserId { get; set; }
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBookingCommandHandler> _logger;

    public CreateBookingCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateBookingCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Validate screening exists and is active
            var screening = await _unitOfWork.Screenings.GetByIdAsync(request.Dto.ScreeningId, cancellationToken);
            if (screening == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<BookingDto>.ErrorResponse("Screening not found");
            }

            if (!screening.IsActive)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<BookingDto>.ErrorResponse("Screening is not active");
            }

            // Validate seats exist, are active, and belong to the same auditorium
            var seats = new List<Seat>();
            foreach (var seatId in request.Dto.SeatIds)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(seatId, cancellationToken);
                if (seat == null)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<BookingDto>.ErrorResponse($"Seat {seatId} not found");
                }

                if (!seat.IsActive)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<BookingDto>.ErrorResponse($"Seat {seat.GetDisplayString()} is not active");
                }

                if (seat.AuditoriumId != screening.AuditoriumId)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ApiResponse<BookingDto>.ErrorResponse($"Seat {seat.GetDisplayString()} does not belong to the screening's auditorium");
                }

                seats.Add(seat);
            }

            // Check for double booking using domain method
            // Note: We need to load bookings with seats for the domain method to work
            var existingBookings = await _unitOfWork.Bookings.FindAsync(
                b => b.ScreeningId == request.Dto.ScreeningId && 
                     (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed),
                cancellationToken);

            var bookedSeatIds = new HashSet<Guid>();
            foreach (var existingBooking in existingBookings)
            {
                var bookingSeats = await _unitOfWork.BookingSeats.FindAsync(
                    bs => bs.BookingId == existingBooking.Id,
                    cancellationToken);

                foreach (var bookingSeat in bookingSeats)
                {
                    bookedSeatIds.Add(bookingSeat.SeatId);
                }
            }

            // Use domain method to check availability
            var conflictingSeats = seats.Where(s => !screening.IsSeatAvailable(s.Id) || bookedSeatIds.Contains(s.Id)).ToList();
            if (conflictingSeats.Any())
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponse<BookingDto>.ErrorResponse(
                    $"The following seats are already booked: {string.Join(", ", conflictingSeats.Select(s => s.GetDisplayString()))}");
            }

            // Calculate total amount using domain value
            var totalAmount = screening.Price * seats.Count;

            // Create booking using domain constructor
            var booking = new Booking(
                request.Dto.ScreeningId,
                request.UserId,
                totalAmount,
                expirationMinutes: 15);

            // Add seats using domain method (before saving)
            foreach (var seat in seats)
            {
                booking.AddSeat(seat.Id);
            }

            await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            // BookingSeats are tracked through the navigation property

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Map to DTO
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

            _logger.LogInformation("Booking created successfully. BookingId: {BookingId}, UserId: {UserId}", booking.Id, request.UserId);

            return ApiResponse<BookingDto>.SuccessResponse(bookingDto, "Booking created successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when creating booking");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResponse<BookingDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return ApiResponse<BookingDto>.ErrorResponse("An error occurred while creating the booking");
        }
    }
}

