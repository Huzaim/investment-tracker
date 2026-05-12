namespace InvestmentTracker.Application.Auth;

public interface IUserRegistrationPort
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> CreateUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}
