using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Bookings.Queries;

public class GetBookingsByUserQuery : IRequest<ApiResponse<List<BookingDto>>>
{
    public Guid UserId { get; set; }
}

public class GetBookingsByUserQueryHandler : IRequestHandler<GetBookingsByUserQuery, ApiResponse<List<BookingDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookingsByUserQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<BookingDto>>> Handle(GetBookingsByUserQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _unitOfWork.Bookings.FindAsync(
            b => b.UserId == request.UserId,
            cancellationToken);

        var bookingDtos = new List<BookingDto>();

        foreach (var booking in bookings.OrderByDescending(b => b.CreatedAt))
        {
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

            // Load related data
            var screening = await _unitOfWork.Screenings.GetByIdAsync(booking.ScreeningId, cancellationToken);
            if (screening != null)
            {
                var movie = await _unitOfWork.Movies.GetByIdAsync(screening.MovieId, cancellationToken);
                var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(screening.AuditoriumId, cancellationToken);

                bookingDto.Screening = new ScreeningDto
                {
                    Id = screening.Id,
                    MovieId = screening.MovieId,
                    AuditoriumId = screening.AuditoriumId,
                    StartTime = screening.StartTime,
                    EndTime = screening.EndTime,
                    Price = screening.Price,
                    IsActive = screening.IsActive,
                    CreatedAt = screening.CreatedAt,
                    Movie = movie != null ? new MovieDto
                    {
                        Id = movie.Id,
                        Title = movie.Title,
                        Description = movie.Description,
                        DurationMinutes = movie.DurationMinutes,
                        Genre = movie.Genre,
                        Director = movie.Director,
                        ReleaseDate = movie.ReleaseDate,
                        PosterUrl = movie.PosterUrl,
                        TicketPrice = movie.TicketPrice,
                        IsActive = movie.IsActive,
                        CreatedAt = movie.CreatedAt
                    } : null,
                    Auditorium = auditorium != null ? new AuditoriumDto
                    {
                        Id = auditorium.Id,
                        Name = auditorium.Name,
                        Capacity = auditorium.Capacity,
                        IsActive = auditorium.IsActive,
                        CreatedAt = auditorium.CreatedAt
                    } : null
                };
            }

            var bookingSeats = await _unitOfWork.BookingSeats.FindAsync(
                bs => bs.BookingId == booking.Id,
                cancellationToken);

            foreach (var bookingSeat in bookingSeats)
            {
                var seat = await _unitOfWork.Seats.GetByIdAsync(bookingSeat.SeatId, cancellationToken);
                bookingDto.BookingSeats.Add(new BookingSeatDto
                {
                    Id = bookingSeat.Id,
                    BookingId = bookingSeat.BookingId,
                    SeatId = bookingSeat.SeatId,
                    Seat = seat != null ? new SeatDto
                    {
                        Id = seat.Id,
                        AuditoriumId = seat.AuditoriumId,
                        Row = seat.Row,
                        Number = seat.Number,
                        IsActive = seat.IsActive,
                        CreatedAt = seat.CreatedAt
                    } : null
                });
            }

            var payment = await _unitOfWork.Payments.FirstOrDefaultAsync(
                p => p.BookingId == booking.Id,
                cancellationToken);

            if (payment != null)
            {
                bookingDto.Payment = new PaymentDto
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
            }

            bookingDtos.Add(bookingDto);
        }

        return ApiResponse<List<BookingDto>>.SuccessResponse(bookingDtos);
    }
}

