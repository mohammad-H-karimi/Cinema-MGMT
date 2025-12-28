using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using MediatR;

namespace Cinema_MGMT.Application.Features.Screenings.Commands;

public class CreateScreeningCommand : IRequest<ApiResponse<ScreeningDto>>
{
    public CreateScreeningDto Dto { get; set; } = null!;
}

public class CreateScreeningCommandHandler : IRequestHandler<CreateScreeningCommand, ApiResponse<ScreeningDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateScreeningCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ScreeningDto>> Handle(CreateScreeningCommand request, CancellationToken cancellationToken)
    {
        // Validate movie exists
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Dto.MovieId, cancellationToken);
        if (movie == null)
        {
            return ApiResponse<ScreeningDto>.ErrorResponse("Movie not found");
        }

        // Validate auditorium exists
        var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(request.Dto.AuditoriumId, cancellationToken);
        if (auditorium == null)
        {
            return ApiResponse<ScreeningDto>.ErrorResponse("Auditorium not found");
        }

        // Calculate end time based on movie duration
        var endTime = request.Dto.StartTime.AddMinutes(movie.DurationMinutes);

        // Create screening using domain constructor
        var screening = new Screening(
            request.Dto.MovieId,
            request.Dto.AuditoriumId,
            request.Dto.StartTime,
            endTime,
            request.Dto.Price);

        await _unitOfWork.Screenings.AddAsync(screening, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var screeningDto = new ScreeningDto
        {
            Id = screening.Id,
            MovieId = screening.MovieId,
            AuditoriumId = screening.AuditoriumId,
            StartTime = screening.StartTime,
            EndTime = screening.EndTime,
            Price = screening.Price,
            IsActive = screening.IsActive,
            CreatedAt = screening.CreatedAt
        };

        return ApiResponse<ScreeningDto>.SuccessResponse(screeningDto, "Screening created successfully");
    }
}

