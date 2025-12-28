using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using Cinema_MGMT.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.Application.Features.Screenings.Commands;

public class UpdateScreeningCommand : IRequest<ApiResponse<ScreeningDto>>
{
    public UpdateScreeningDto Dto { get; set; } = null!;
}

public class UpdateScreeningCommandHandler : IRequestHandler<UpdateScreeningCommand, ApiResponse<ScreeningDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateScreeningCommandHandler> _logger;

    public UpdateScreeningCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateScreeningCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse<ScreeningDto>> Handle(UpdateScreeningCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var screening = await _unitOfWork.Screenings.GetByIdAsync(request.Dto.Id, cancellationToken);
            if (screening == null)
            {
                return ApiResponse<ScreeningDto>.ErrorResponse("Screening not found");
            }

            // Validate movie exists if changed
            if (screening.MovieId != request.Dto.MovieId)
            {
                var movie = await _unitOfWork.Movies.GetByIdAsync(request.Dto.MovieId, cancellationToken);
                if (movie == null)
                {
                    return ApiResponse<ScreeningDto>.ErrorResponse("Movie not found");
                }
            }

            // Validate auditorium exists if changed
            if (screening.AuditoriumId != request.Dto.AuditoriumId)
            {
                var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Dto.AuditoriumId, cancellationToken);
                if (auditorium == null)
                {
                    return ApiResponse<ScreeningDto>.ErrorResponse("Auditorium not found");
                }
            }

            // Note: Since Screening entity doesn't have a full Update method,
            // we'll need to work with what's available. For now, we'll update price and active status.
            // For a full update, you'd need to add an Update method to the domain entity.
            if (screening.Price != request.Dto.Price)
            {
                screening.UpdatePrice(request.Dto.Price);
            }

            if (screening.IsActive != request.Dto.IsActive)
            {
                if (request.Dto.IsActive)
                    screening.Activate();
                else
                    screening.Deactivate();
            }

            await _unitOfWork.Screenings.UpdateAsync(screening, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var movie = await _unitOfWork.Movies.GetByIdAsync(screening.MovieId, cancellationToken);
            var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(screening.AuditoriumId, cancellationToken);

            var screeningDto = new ScreeningDto
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

            _logger.LogInformation("Screening updated successfully. ScreeningId: {ScreeningId}", screening.Id);

            return ApiResponse<ScreeningDto>.SuccessResponse(screeningDto, "Screening updated successfully");
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation failed when updating screening");
            return ApiResponse<ScreeningDto>.ErrorResponse(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating screening");
            return ApiResponse<ScreeningDto>.ErrorResponse("An error occurred while updating the screening");
        }
    }
}

