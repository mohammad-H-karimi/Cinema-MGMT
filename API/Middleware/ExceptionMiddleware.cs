using System.Net;
using System.Text.Json;
using Cinema_MGMT.Application.DTOs;
using Cinema_MGMT.Application.Exceptions;
using Cinema_MGMT.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cinema_MGMT.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. RequestId: {RequestId}", context.TraceIdentifier);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";
        var errors = new List<string> { exception.Message };

        // Handle validation exceptions
        if (exception is ValidationException validationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = "Validation failed";
            errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
        }
        // Handle domain exceptions (business rule violations)
        else if (exception is DomainException domainException)
        {
            statusCode = HttpStatusCode.BadRequest; // 400 Bad Request for domain violations
            message = domainException.Message;
            // Log as warning since these are expected business rule violations
        }
        // Handle application exceptions (API-level errors)
        else if (exception is ApiException apiException)
        {
            statusCode = (HttpStatusCode)apiException.StatusCode;
            message = apiException.Message;
        }
        // Handle other exceptions (unexpected errors)
        else
        {
            // Log full exception details for unexpected errors
            // In production, you might want to return a generic message
            message = "An unexpected error occurred. Please try again later.";
        }

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.ErrorResponse(message, errors);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}

