using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InvestmentTracker.Application.Auth;
using InvestmentTracker.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace InvestmentTracker.Infrastructure.Auth;

public sealed class IdentityUserAuthenticationPort(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions) : IUserAuthenticationPort
{
    public async Task<LoginUserResult> AuthenticateAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            return LoginUserResult.Failure("Invalid email or password.");
        }

        var passwordMatches = await userManager.CheckPasswordAsync(user, command.Password);
        if (!passwordMatches)
        {
            return LoginUserResult.Failure("Invalid email or password.");
        }

        var settings = jwtOptions.Value;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(settings.AccessTokenMinutes);

        var token = CreateToken(user, settings, expiresAt);

        return LoginUserResult.Success(token, expiresAt);
    }

    private static string CreateToken(
        ApplicationUser user,
        JwtOptions settings,
        DateTimeOffset expiresAt)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
