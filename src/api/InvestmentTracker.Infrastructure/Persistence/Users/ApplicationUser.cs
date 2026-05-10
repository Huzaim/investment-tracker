using Microsoft.AspNetCore.Identity;

namespace InvestmentTracker.Infrastructure.Persistence.Users;

public sealed class ApplicationUser : IdentityUser<Guid>
{
}
