using InvestmentTracker.Api.Contracts.Auth;
using InvestmentTracker.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<RegisterUserSuccessResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var result = await sender.Send(command, HttpContext.RequestAborted);

        if (!result.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Unable to register user.",
                Extensions =
                {
                    ["errors"] = result.Errors
                }
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new RegisterUserSuccessResponse("User registered successfully."));
    }

    [HttpPost("login")]
    [ProducesResponseType<LoginUserSuccessResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await sender.Send(command, HttpContext.RequestAborted);

        if (!result.Succeeded || result.AccessToken is null || result.ExpiresAt is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Invalid email or password."
            });
        }

        return Ok(new LoginUserSuccessResponse(
            result.AccessToken,
            result.ExpiresAt.Value));
    }
}
