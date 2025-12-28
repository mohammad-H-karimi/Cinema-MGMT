using System.Security.Claims;
using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Bookings.Commands;
using Cinema_MGMT.Application.Features.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IMediator mediator, ILogger<BookingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all bookings for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<BookingDto>>>> GetMyBookings(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var query = new GetBookingsByUserQuery { UserId = userId.Value };
        var result = await _mediator.Send(query, cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Get a specific booking by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetBookingById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var query = new GetBookingByIdQuery { Id = id, UserId = userId.Value };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateBooking([FromBody] CreateBookingDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var command = new CreateBookingCommand { Dto = dto, UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetBookingById), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Confirm a pending booking
    /// </summary>
    [HttpPost("{id}/confirm")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> ConfirmBooking(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var command = new ConfirmBookingCommand { BookingId = id, UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            // Check if it's a not found error
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CancelBooking(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var command = new CancelBookingCommand { BookingId = id, UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
        {
            // Check if it's a not found error
            if (result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}

