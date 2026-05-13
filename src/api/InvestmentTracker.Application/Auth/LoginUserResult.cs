namespace InvestmentTracker.Application.Auth;

public sealed record LoginUserResult(
    bool Succeeded,
    string? AccessToken,
    DateTimeOffset? ExpiresAt,
    IReadOnlyCollection<string> Errors)
{
    public static LoginUserResult Success(string accessToken, DateTimeOffset expiresAt) =>
        new(true, accessToken, expiresAt, Array.Empty<string>());

    public static LoginUserResult Failure(params string[] errors) =>
        new(false, null, null, errors);
}
