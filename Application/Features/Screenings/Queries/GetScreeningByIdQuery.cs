using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Screenings.Queries;

public class GetScreeningByIdQuery : IRequest<ApiResponse<ScreeningDto>>
{
    public Guid Id { get; set; }
}

public class GetScreeningByIdQueryHandler : IRequestHandler<GetScreeningByIdQuery, ApiResponse<ScreeningDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetScreeningByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ScreeningDto>> Handle(GetScreeningByIdQuery request, CancellationToken cancellationToken)
    {
        var screening = await _unitOfWork.Screenings.GetByIdAsync(request.Id, cancellationToken);

        if (screening == null)
        {
            return ApiResponse<ScreeningDto>.ErrorResponse("Screening not found");
        }

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

        return ApiResponse<ScreeningDto>.SuccessResponse(screeningDto);
    }
}

