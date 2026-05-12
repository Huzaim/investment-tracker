namespace InvestmentTracker.Application.Auth;

public sealed record RegisterUserResult(bool Succeeded, IReadOnlyCollection<string> Errors)
{
    public static RegisterUserResult Success() => new(true, Array.Empty<string>());

    public static RegisterUserResult Failure(params string[] errors) => new(false, errors);
}
