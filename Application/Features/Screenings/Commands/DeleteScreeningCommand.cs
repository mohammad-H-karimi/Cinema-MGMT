using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Screenings.Commands;

public class DeleteScreeningCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteScreeningCommandHandler : IRequestHandler<DeleteScreeningCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteScreeningCommandHandler> _logger;

    public DeleteScreeningCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteScreeningCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteScreeningCommand request, CancellationToken cancellationToken)
    {
        var screening = await _unitOfWork.Screenings.GetByIdAsync(request.Id, cancellationToken);
        if (screening == null)
        {
            return ApiResponse<bool>.ErrorResponse("Screening not found");
        }

        // Check if there are any active bookings
        var activeBookings = await _unitOfWork.Bookings.FindAsync(
            b => b.ScreeningId == request.Id && 
                 (b.Status == Domain.Enums.BookingStatus.Pending || b.Status == Domain.Enums.BookingStatus.Confirmed),
            cancellationToken);

        if (activeBookings.Any())
        {
            return ApiResponse<bool>.ErrorResponse("Cannot delete screening with active bookings");
        }

        // Deactivate instead of delete (soft delete)
        screening.Deactivate();
        await _unitOfWork.Screenings.UpdateAsync(screening, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Screening deleted (deactivated) successfully. ScreeningId: {ScreeningId}", request.Id);

        return ApiResponse<bool>.SuccessResponse(true, "Screening deleted successfully");
    }
}

