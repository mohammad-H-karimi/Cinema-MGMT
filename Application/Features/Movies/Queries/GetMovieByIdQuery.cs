using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Movies.Queries;

public class GetMovieByIdQuery : IRequest<ApiResponse<MovieDto>>
{
    public Guid Id { get; set; }
}

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, ApiResponse<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<MovieDto>> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Id, cancellationToken);

        if (movie == null)
        {
            return ApiResponse<MovieDto>.ErrorResponse("Movie not found");
        }

        var movieDto = new MovieDto
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
        };

        return ApiResponse<MovieDto>.SuccessResponse(movieDto);
    }
}

