using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Cinema_MGMT.Domain.Entities;
using MediatR;

namespace Cinema_MGMT.Application.Features.Movies.Commands;

public class CreateMovieCommand : IRequest<ApiResponse<MovieDto>>
{
    public CreateMovieDto Dto { get; set; } = null!;
}

public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, ApiResponse<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<MovieDto>> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        // Create movie using domain constructor
        var movie = new Movie(
            request.Dto.Title,
            request.Dto.Description,
            request.Dto.DurationMinutes,
            request.Dto.Genre,
            request.Dto.Director,
            request.Dto.ReleaseDate,
            request.Dto.TicketPrice,
            request.Dto.PosterUrl);

        await _unitOfWork.Movies.AddAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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

        return ApiResponse<MovieDto>.SuccessResponse(movieDto, "Movie created successfully");
    }
}

