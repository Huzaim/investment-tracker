namespace InvestmentTracker.Application.Auth;

public sealed class RegisterUserCommandHandler(IUserRegistrationPort userRegistrationPort)
    : IRegisterUserCommandHandler
{
    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await userRegistrationPort.EmailExistsAsync(command.Email, cancellationToken))
        {
            return RegisterUserResult.Failure("Email is already registered.");
        }

        var errors = await userRegistrationPort.CreateUserAsync(
            command.Email,
            command.Password,
            cancellationToken);

        if (errors.Count > 0)
        {
            return RegisterUserResult.Failure(errors.ToArray());
        }

        return RegisterUserResult.Success();
    }
}
