namespace InvestmentTracker.Application.Auth;

public interface IUserAuthenticationPort
{
    Task<LoginUserResult> AuthenticateAsync(LoginUserCommand command, CancellationToken cancellationToken);
}
