using InvestmentTracker.Application.Auth;
using InvestmentTracker.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Unable to register user.",
                errors = result.Errors.Select(error => error.Description)
            });
        }

        return StatusCode(StatusCodes.Status201Created);
    }
}
