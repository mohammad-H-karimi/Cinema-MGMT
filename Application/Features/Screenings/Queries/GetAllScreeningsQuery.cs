using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Screenings.Queries;

public class GetAllScreeningsQuery : IRequest<ApiResponse<List<ScreeningDto>>>
{
}

public class GetAllScreeningsQueryHandler : IRequestHandler<GetAllScreeningsQuery, ApiResponse<List<ScreeningDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllScreeningsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<ScreeningDto>>> Handle(GetAllScreeningsQuery request, CancellationToken cancellationToken)
    {
        var screenings = await _unitOfWork.Screenings.GetAllAsync(cancellationToken);
        
        var screeningDtos = new List<ScreeningDto>();
        foreach (var screening in screenings)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(screening.MovieId, cancellationToken);
            var auditorium = await _unitOfWork.Auditoriums.GetByIdAsync(screening.AuditoriumId, cancellationToken);

            screeningDtos.Add(new ScreeningDto
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
            });
        }

        return ApiResponse<List<ScreeningDto>>.SuccessResponse(screeningDtos);
    }
}

