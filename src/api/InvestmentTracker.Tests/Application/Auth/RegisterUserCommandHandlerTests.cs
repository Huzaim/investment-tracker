using InvestmentTracker.Application.Auth;

namespace InvestmentTracker.Tests.Application.Auth;

public sealed class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserIsCreated()
    {
        var port = new FakeUserRegistrationPort { ErrorsToReturn = Array.Empty<string>() };
        var handler = new RegisterUserCommandHandler(port);
        var command = new RegisterUserCommand("user@example.com", "P@ssw0rd!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
        Assert.Equal("user@example.com", port.Email);
        Assert.Equal("P@ssw0rd!", port.Password);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenUserCreationFails()
    {
        var port = new FakeUserRegistrationPort { ErrorsToReturn = ["Email already exists."] };
        var handler = new RegisterUserCommandHandler(port);
        var command = new RegisterUserCommand("user@example.com", "P@ssw0rd!");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(["Email already exists."], result.Errors);
    }

    [Fact]
    public async Task Handle_ThrowsWhenCancellationIsRequested()
    {
        var port = new FakeUserRegistrationPort();
        var handler = new RegisterUserCommandHandler(port);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(
                new RegisterUserCommand("user@example.com", "P@ssw0rd!"),
                cts.Token));

        Assert.False(port.WasCalled);
    }

    private sealed class FakeUserRegistrationPort : IUserRegistrationPort
    {
        public string? Email { get; private set; }

        public string? Password { get; private set; }

        public bool WasCalled { get; private set; }

        public IReadOnlyCollection<string> ErrorsToReturn { get; set; } = Array.Empty<string>();

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<string>> CreateUserAsync(
            string email,
            string password,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            Email = email;
            Password = password;

            return Task.FromResult(ErrorsToReturn);
        }
    }
}
