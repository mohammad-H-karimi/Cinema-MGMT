using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Movies.Commands;
using Cinema_MGMT.Application.Features.Movies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMediator mediator, ILogger<MoviesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<MovieDto>>>> GetAllMovies(CancellationToken cancellationToken)
    {
        var query = new GetAllMoviesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> GetMovieById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetMovieByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> CreateMovie([FromBody] CreateMovieDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateMovieCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetMovieById), new { id = result.Data!.Id }, result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<MovieDto>>> UpdateMovie([FromBody] UpdateMovieDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateMovieCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteMovie(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteMovieCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }
}

