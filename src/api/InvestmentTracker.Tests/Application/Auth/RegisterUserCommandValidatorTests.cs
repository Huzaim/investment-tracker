using InvestmentTracker.Application.Auth;

namespace InvestmentTracker.Tests.Application.Auth;

public sealed class RegisterUserCommandValidatorTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsError_WhenEmailAlreadyExists()
    {
        var port = new FakeUserRegistrationPort(emailExists: true);
        var validator = new RegisterUserCommandValidator(port);

        var result = await validator.ValidateAsync(
            new RegisterUserCommand("user@example.com", "P@ssw0rd!"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RegisterUserCommand.Email) &&
            error.ErrorMessage == "Email is already registered.");
    }

    [Fact]
    public async Task ValidateAsync_ReturnsValid_WhenEmailIsUnique()
    {
        var port = new FakeUserRegistrationPort(emailExists: false);
        var validator = new RegisterUserCommandValidator(port);

        var result = await validator.ValidateAsync(
            new RegisterUserCommand("user@example.com", "P@ssw0rd!"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_AllowsBlankEmail_AndSkipsLookup()
    {
        var port = new FakeUserRegistrationPort(emailExists: true);
        var validator = new RegisterUserCommandValidator(port);

        var result = await validator.ValidateAsync(
            new RegisterUserCommand(" ", "P@ssw0rd!"));

        Assert.True(result.IsValid);
        Assert.False(port.WasCalled);
    }

    private sealed class FakeUserRegistrationPort : IUserRegistrationPort
    {
        private readonly bool emailExists;

        public FakeUserRegistrationPort(bool emailExists)
        {
            this.emailExists = emailExists;
        }

        public bool WasCalled { get; private set; }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult(emailExists);
        }

        public Task<IReadOnlyCollection<string>> CreateUserAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
