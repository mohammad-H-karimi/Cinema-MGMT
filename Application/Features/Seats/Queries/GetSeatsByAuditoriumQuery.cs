using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Seats.Queries;

public class GetSeatsByAuditoriumQuery : IRequest<ApiResponse<List<SeatDto>>>
{
    public Guid AuditoriumId { get; set; }
}

public class GetSeatsByAuditoriumQueryHandler : IRequestHandler<GetSeatsByAuditoriumQuery, ApiResponse<List<SeatDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSeatsByAuditoriumQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<SeatDto>>> Handle(GetSeatsByAuditoriumQuery request, CancellationToken cancellationToken)
    {
        var seats = await _unitOfWork.Seats.FindAsync(s => s.AuditoriumId == request.AuditoriumId, cancellationToken);
        
        var seatDtos = seats.Select(s => new SeatDto
        {
            Id = s.Id,
            AuditoriumId = s.AuditoriumId,
            Row = s.Row,
            Number = s.Number,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        }).ToList();

        return ApiResponse<List<SeatDto>>.SuccessResponse(seatDtos);
    }
}

