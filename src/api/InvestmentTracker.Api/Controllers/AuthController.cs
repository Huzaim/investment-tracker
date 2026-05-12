using InvestmentTracker.Api.Contracts.Auth;
using InvestmentTracker.Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<RegisterUserSuccessResponse>(StatusCodes.Status201Created)]
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
}
