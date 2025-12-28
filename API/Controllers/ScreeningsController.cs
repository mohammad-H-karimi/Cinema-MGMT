using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Screenings.Commands;
using Cinema_MGMT.Application.Features.Screenings.Queries;
using Cinema_MGMT.Application.Features.Seats.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScreeningsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ScreeningsController> _logger;

    public ScreeningsController(IMediator mediator, ILogger<ScreeningsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all screenings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<ScreeningDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ScreeningDto>>>> GetAllScreenings(CancellationToken cancellationToken)
    {
        var query = new GetAllScreeningsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific screening by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> GetScreeningById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetScreeningByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get available seats for a screening
    /// </summary>
    [HttpGet("{id}/seats")]
    [ProducesResponseType(typeof(ApiResponse<List<SeatDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<SeatDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<SeatDto>>>> GetScreeningSeats(Guid id, CancellationToken cancellationToken)
    {
        var screeningQuery = new GetScreeningByIdQuery { Id = id };
        var screeningResult = await _mediator.Send(screeningQuery, cancellationToken);
        
        if (!screeningResult.Success || screeningResult.Data == null)
            return NotFound(ApiResponse<List<SeatDto>>.ErrorResponse("Screening not found"));

        var seatsQuery = new GetSeatsByAuditoriumQuery { AuditoriumId = screeningResult.Data.AuditoriumId };
        var seatsResult = await _mediator.Send(seatsQuery, cancellationToken);
        
        return Ok(seatsResult);
    }

    /// <summary>
    /// Create a new screening
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> CreateScreening([FromBody] CreateScreeningDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateScreeningCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetScreeningById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update a screening
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<ScreeningDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ScreeningDto>>> UpdateScreening([FromBody] UpdateScreeningDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateScreeningCommand { Dto = dto };
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
    /// Delete a screening
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteScreening(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteScreeningCommand { Id = id };
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

