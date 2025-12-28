using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Auditoriums.Commands;

public class DeleteAuditoriumCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteAuditoriumCommandHandler : IRequestHandler<DeleteAuditoriumCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAuditoriumCommandHandler> _logger;

    public DeleteAuditoriumCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteAuditoriumCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteAuditoriumCommand request, CancellationToken cancellationToken)
    {
        var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Id, cancellationToken);
        if (auditorium == null)
        {
            return ApiResponse<bool>.ErrorResponse("Auditorium not found");
        }

        // Check if there are any active screenings
        var activeScreenings = await _unitOfWork.Screenings.FindAsync(
            s => s.AuditoriumId == request.Id && s.IsActive,
            cancellationToken);

        if (activeScreenings.Any())
        {
            return ApiResponse<bool>.ErrorResponse("Cannot delete auditorium with active screenings");
        }

        // Deactivate instead of delete (soft delete)
        auditorium.Deactivate();
        await _unitOfWork.Auditoriums.UpdateAsync(auditorium, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Auditorium deleted (deactivated) successfully. AuditoriumId: {AuditoriumId}", request.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Auditorium deleted successfully");
    }
}

