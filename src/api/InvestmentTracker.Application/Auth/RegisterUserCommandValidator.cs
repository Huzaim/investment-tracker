using FluentValidation;

namespace InvestmentTracker.Application.Auth;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRegistrationPort userRegistrationPort;

    public RegisterUserCommandValidator(IUserRegistrationPort userRegistrationPort)
    {
        this.userRegistrationPort = userRegistrationPort;

        RuleFor(x => x.Email)
            .MustAsync(BeUniqueEmailAsync)
            .WithMessage("Email is already registered.");
    }

    private async Task<bool> BeUniqueEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }

        return !await userRegistrationPort.EmailExistsAsync(email, cancellationToken);
    }
}
