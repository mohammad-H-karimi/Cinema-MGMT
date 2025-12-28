using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Auditoriums.Queries;

public class GetAuditoriumByIdQuery : IRequest<ApiResponse<AuditoriumDto>>
{
    public Guid Id { get; set; }
}

public class GetAuditoriumByIdQueryHandler : IRequestHandler<GetAuditoriumByIdQuery, ApiResponse<AuditoriumDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditoriumByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<AuditoriumDto>> Handle(GetAuditoriumByIdQuery request, CancellationToken cancellationToken)
    {
        var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Id, cancellationToken);

        if (auditorium == null)
        {
            return ApiResponse<AuditoriumDto>.ErrorResponse("Auditorium not found");
        }

        var auditoriumDto = new AuditoriumDto
        {
            Id = auditorium.Id,
            Name = auditorium.Name,
            Capacity = auditorium.Capacity,
            IsActive = auditorium.IsActive,
            CreatedAt = auditorium.CreatedAt
        };

        return ApiResponse<AuditoriumDto>.SuccessResponse(auditoriumDto);
    }
}

