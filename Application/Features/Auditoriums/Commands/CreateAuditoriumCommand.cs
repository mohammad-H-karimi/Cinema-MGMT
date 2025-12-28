using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using MediatR;

namespace Cinema_MGMT.Application.Features.Auditoriums.Commands;

public class CreateAuditoriumCommand : IRequest<ApiResponse<AuditoriumDto>>
{
    public CreateAuditoriumDto Dto { get; set; } = null!;
}

public class CreateAuditoriumCommandHandler : IRequestHandler<CreateAuditoriumCommand, ApiResponse<AuditoriumDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuditoriumCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<AuditoriumDto>> Handle(CreateAuditoriumCommand request, CancellationToken cancellationToken)
    {
        // Create auditorium using domain constructor
        var auditorium = new Auditorium(
            request.Dto.Name,
            request.Dto.Capacity);

        await _unitOfWork.Auditoriums.AddAsync(auditorium, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var auditoriumDto = new AuditoriumDto
        {
            Id = auditorium.Id,
            Name = auditorium.Name,
            Capacity = auditorium.Capacity,
            IsActive = auditorium.IsActive,
            CreatedAt = auditorium.CreatedAt
        };

        return ApiResponse<AuditoriumDto>.SuccessResponse(auditoriumDto, "Auditorium created successfully");
    }
}

