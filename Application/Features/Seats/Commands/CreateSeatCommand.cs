using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using MediatR;

namespace Cinema_MGMT.Application.Features.Seats.Commands;

public class CreateSeatCommand : IRequest<ApiResponse<SeatDto>>
{
    public CreateSeatDto Dto { get; set; } = null!;
}

public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, ApiResponse<SeatDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSeatCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SeatDto>> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
    {
        // Validate auditorium exists
        var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Dto.AuditoriumId, cancellationToken);
        if (auditorium == null)
        {
            return ApiResponse<SeatDto>.ErrorResponse("Auditorium not found");
        }

        // Check if seat already exists
        var existingSeat = await _unitOfWork.Seats.FirstOrDefaultAsync(
            s => s.AuditoriumId == request.Dto.AuditoriumId && 
                 s.Row == request.Dto.Row && 
                 s.Number == request.Dto.Number,
            cancellationToken);

        if (existingSeat != null)
        {
            return ApiResponse<SeatDto>.ErrorResponse("Seat already exists in this auditorium");
        }

        // Create seat using domain constructor
        var seat = new Seat(
            request.Dto.AuditoriumId,
            request.Dto.Row,
            request.Dto.Number);

        await _unitOfWork.Seats.AddAsync(seat, cancellationToken);
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

        return ApiResponse<SeatDto>.SuccessResponse(seatDto, "Seat created successfully");
    }
}

