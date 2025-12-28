using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Auditoriums.Commands;

public class UpdateAuditoriumCommand : IRequest<ApiResponse<AuditoriumDto>>
{
    public UpdateAuditoriumDto Dto { get; set; } = null!;
}

public class UpdateAuditoriumCommandHandler : IRequestHandler<UpdateAuditoriumCommand, ApiResponse<AuditoriumDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAuditoriumCommandHandler> _logger;

    public UpdateAuditoriumCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateAuditoriumCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<AuditoriumDto>> Handle(UpdateAuditoriumCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (auditorium == null)
            {
                return ApiResponse<AuditoriumDto>.ErrorResponse("Auditorium not found");
            }

            auditorium.Update(request.Dto.Name, request.Dto.Capacity);

            if (auditorium.IsActive != request.Dto.IsActive)
            {
                if (request.Dto.IsActive)
                    auditorium.Activate();
                else
                    auditorium.Deactivate();
            }

            await _unitOfWork.Auditoriums.UpdateAsync(auditorium, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var auditoriumDto = new AuditoriumDto
            {
                Id = auditorium.Id,
                Name = auditorium.Name,
                Capacity = auditorium.Capacity,
                IsActive = auditorium.IsActive,
                CreatedAt = auditorium.CreatedAt
            };

            _logger.LogInformation("Auditorium updated successfully. AuditoriumId: {AuditoriumId}", auditorium.Id);

            return ApiResponse<AuditoriumDto>.SuccessResponse(auditoriumDto, "Auditorium updated successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when updating auditorium");
            return ApiResponse<AuditoriumDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating auditorium");
            return ApiResponse<AuditoriumDto>.ErrorResponse("An error occurred while updating the auditorium");
        }
    }
}

