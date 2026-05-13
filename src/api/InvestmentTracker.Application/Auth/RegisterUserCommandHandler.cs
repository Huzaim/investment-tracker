using MediatR;

namespace InvestmentTracker.Application.Auth;

public sealed class RegisterUserCommandHandler(IUserRegistrationPort userRegistrationPort)
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var errors = await userRegistrationPort.CreateUserAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (errors.Count > 0)
        {
            return RegisterUserResult.Failure(errors.ToArray());
        }

        return RegisterUserResult.Success();
    }
}
