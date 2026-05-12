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
    [ProducesResponseType<RegisterUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<RegisterUserResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var result = await sender.Send(command, HttpContext.RequestAborted);

        if (!result.Succeeded)
        {
            return BadRequest(new RegisterUserResponse(
                false,
                "Unable to register user.",
                result.Errors));
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new RegisterUserResponse(true, "User registered successfully.", Array.Empty<string>()));
    }
}
