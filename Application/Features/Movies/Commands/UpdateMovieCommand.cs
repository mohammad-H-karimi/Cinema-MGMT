using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Movies.Commands;

public class UpdateMovieCommand : IRequest<ApiResponse<MovieDto>>
{
    public UpdateMovieDto Dto { get; set; } = null!;
}

public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, ApiResponse<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<MovieDto>> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Dto.Id, cancellationToken);

        if (movie == null)
        {
            return ApiResponse<MovieDto>.ErrorResponse("Movie not found");
        }

        // Update movie using domain method
        movie.Update(
            title: request.Dto.Title,
            description: request.Dto.Description,
            durationMinutes: request.Dto.DurationMinutes,
            genre: request.Dto.Genre,
            director: request.Dto.Director,
            releaseDate: request.Dto.ReleaseDate,
            ticketPrice: request.Dto.TicketPrice,
            posterUrl: request.Dto.PosterUrl);

        // Update active status if changed
        if (request.Dto.IsActive && !movie.IsActive)
        {
            movie.Activate();
        }
        else if (!request.Dto.IsActive && movie.IsActive)
        {
            movie.Deactivate();
        }

        await _unitOfWork.Movies.UpdateAsync(movie, cancellationToken);
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

        return ApiResponse<MovieDto>.SuccessResponse(movieDto, "Movie updated successfully");
    }
}

