using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Seats.Queries;

public class GetSeatByIdQuery : IRequest<ApiResponse<SeatDto>>
{
    public Guid Id { get; set; }
}

public class GetSeatByIdQueryHandler : IRequestHandler<GetSeatByIdQuery, ApiResponse<SeatDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSeatByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SeatDto>> Handle(GetSeatByIdQuery request, CancellationToken cancellationToken)
    {
        var seat = await _unitOfWork.Seats.GetByIdAsync(request.Id, cancellationToken);

        if (seat == null)
        {
            return ApiResponse<SeatDto>.ErrorResponse("Seat not found");
        }

        var seatDto = new SeatDto
        {
            Id = seat.Id,
            AuditoriumId = seat.AuditoriumId,
            Row = seat.Row,
            Number = seat.Number,
            IsActive = seat.IsActive,
            CreatedAt = seat.CreatedAt
        };

        return ApiResponse<SeatDto>.SuccessResponse(seatDto);
    }
}

