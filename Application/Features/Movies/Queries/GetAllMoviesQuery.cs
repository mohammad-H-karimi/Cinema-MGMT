using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Movies.Queries;

public class GetAllMoviesQuery : IRequest<ApiResponse<List<MovieDto>>>
{
}

public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, ApiResponse<List<MovieDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllMoviesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<MovieDto>>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var movies = await _unitOfWork.Movies.GetAllAsync(cancellationToken);
        
        var movieDtos = movies.Select(m => new MovieDto
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            DurationMinutes = m.DurationMinutes,
            Genre = m.Genre,
            Director = m.Director,
            ReleaseDate = m.ReleaseDate,
            PosterUrl = m.PosterUrl,
            TicketPrice = m.TicketPrice,
            IsActive = m.IsActive,
            CreatedAt = m.CreatedAt
        }).ToList();

        return ApiResponse<List<MovieDto>>.SuccessResponse(movieDtos);
    }
}

