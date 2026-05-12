using InvestmentTracker.Application.Auth;
using InvestmentTracker.Infrastructure.Persistence.Users;
using Microsoft.AspNetCore.Identity;

namespace InvestmentTracker.Infrastructure.Auth;

public sealed class IdentityUserRegistrationPort(
    UserManager<ApplicationUser> userManager) : IUserRegistrationPort
{
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var existingUser = await userManager.FindByEmailAsync(email);
        return existingUser is not null;
    }

    public async Task<IReadOnlyCollection<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return Array.Empty<string>();
        }

        return result.Errors.Select(error => error.Description).ToArray();
    }
}
