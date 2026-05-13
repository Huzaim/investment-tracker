using MediatR;

namespace InvestmentTracker.Application.Auth;

public sealed class LoginUserCommandHandler(IUserAuthenticationPort authenticationPort)
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public Task<LoginUserResult> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        return authenticationPort.AuthenticateAsync(request, cancellationToken);
    }
}
