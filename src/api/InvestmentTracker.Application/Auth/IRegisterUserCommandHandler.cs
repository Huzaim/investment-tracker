namespace InvestmentTracker.Application.Auth;

public interface IRegisterUserCommandHandler
{
    Task<RegisterUserResult> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken);
}
