namespace InvestmentTracker.Api.Contracts.Auth;

public sealed record RegisterUserResponse(
    bool Succeeded,
    string Message,
    IReadOnlyCollection<string> Errors);
