using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Auditoriums.Commands;
using Cinema_MGMT.Application.Features.Auditoriums.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditoriumsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuditoriumsController> _logger;

    public AuditoriumsController(IMediator mediator, ILogger<AuditoriumsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all auditoriums
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AuditoriumDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AuditoriumDto>>>> GetAllAuditoriums(CancellationToken cancellationToken)
    {
        var query = new GetAllAuditoriumsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific auditorium by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AuditoriumDto>>> GetAuditoriumById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAuditoriumByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new auditorium
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuditoriumDto>>> CreateAuditorium([FromBody] CreateAuditoriumDto dto, CancellationToken cancellationToken)
    {
        var command = new CreateAuditoriumCommand { Dto = dto };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetAuditoriumById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Update an auditorium
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuditoriumDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AuditoriumDto>>> UpdateAuditorium([FromBody] UpdateAuditoriumDto dto, CancellationToken cancellationToken)
    {
        var command = new UpdateAuditoriumCommand { Dto = dto };
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
    /// Delete an auditorium
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteAuditorium(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAuditoriumCommand { Id = id };
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

