using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_MGMT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtService jwtService, ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // In a real application, validate credentials against database
        // For demo purposes, we'll accept any email/password
        if (string.IsNullOrEmpty(request.Email))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Email is required"));
        }

        // Generate token (in production, validate user credentials first)
        var userId = Guid.NewGuid(); // In production, get from authenticated user
        var token = _jwtService.GenerateToken(userId, request.Email, new List<string> { "User" });

        _logger.LogInformation("User logged in: {Email}", request.Email);

        return Ok(ApiResponse<LoginResponse>.SuccessResponse(new LoginResponse
        {
            Token = token,
            Email = request.Email
        }, "Login successful"));
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            UserId = userId,
            Email = email
        }));
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

