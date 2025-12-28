using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Seats.Commands;
using Cinema_MGMT.Application.Features.Seats.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeatsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SeatsController> _logger;

    public SeatsController(IMediator mediator, ILogger<SeatsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all seats for an auditorium
    /// </summary>
    [HttpGet("auditorium/{auditoriumId}")]
    [ProducesResponseType(typeof(ApiResponse<List<SeatDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<SeatDto>>>> GetSeatsByAuditorium(Guid auditoriumId, CancellationToken cancellationToken)
    {
        var query = new GetSeatsByAuditoriumQuery { AuditoriumId = auditoriumId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific seat by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SeatDto>>> GetSeatById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetSeatByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new seat
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<SeatDto>>> CreateSeat([FromBody] CreateSeatDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateSeatCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetSeatById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update a seat
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SeatDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SeatDto>>> UpdateSeat([FromBody] UpdateSeatDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateSeatCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a seat
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteSeat(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSeatCommand { Id = id };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }
}

