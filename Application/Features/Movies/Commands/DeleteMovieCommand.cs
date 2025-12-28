using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using MediatR;

namespace Cinema_MGMT.Application.Features.Movies.Commands;

public class DeleteMovieCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}

public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, ApiResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Id, cancellationToken);

        if (movie == null)
        {
            return ApiResponse<bool>.ErrorResponse("Movie not found");
        }

        await _unitOfWork.Movies.DeleteAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<bool>.SuccessResponse(true, "Movie deleted successfully");
    }
}

