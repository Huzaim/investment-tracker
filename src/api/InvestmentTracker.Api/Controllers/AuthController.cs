using InvestmentTracker.Application.Auth;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IRegisterUserCommandHandler registerUserCommandHandler) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserCommand request)
    {
        var result = await registerUserCommandHandler.HandleAsync(request, HttpContext.RequestAborted);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Unable to register user.",
                errors = result.Errors
            });
        }

        return StatusCode(StatusCodes.Status201Created);
    }
}
