using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Auditoriums.Queries;

public class GetAllAuditoriumsQuery : IRequest<ApiResponse<List<AuditoriumDto>>>
{
}

public class GetAllAuditoriumsQueryHandler : IRequestHandler<GetAllAuditoriumsQuery, ApiResponse<List<AuditoriumDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllAuditoriumsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<AuditoriumDto>>> Handle(GetAllAuditoriumsQuery request, CancellationToken cancellationToken)
    {
        var auditoriums = await _unitOfWork.Auditoriums.GetAllAsync(cancellationToken);
        
        var auditoriumDtos = auditoriums.Select(a => new AuditoriumDto
        {
            Id = a.Id,
            Name = a.Name,
            Capacity = a.Capacity,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt
        }).ToList();

        return ApiResponse<List<AuditoriumDto>>.SuccessResponse(auditoriumDtos);
    }
}

