using System.Security.Claims;
using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Features.Payments.Commands;
using Cinema_MGMT.Application.Features.Payments.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get a specific payment by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var query = new GetPaymentByIdQuery { Id = id, UserId = userId.Value };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Get all payments for a booking
    /// </summary>
    [HttpGet("booking/{bookingId}")]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<PaymentDto>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<PaymentDto>>>> GetPaymentsByBooking(Guid bookingId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var query = new GetPaymentsByBookingQuery { BookingId = bookingId, UserId = userId.Value };
        var result = await _mediator.Send(query, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> CreatePayment([FromBody] CreatePaymentDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var command = new CreatePaymentCommand { Dto = dto, UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetPaymentById), new { id = result.Data!.Id }, result);
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}

