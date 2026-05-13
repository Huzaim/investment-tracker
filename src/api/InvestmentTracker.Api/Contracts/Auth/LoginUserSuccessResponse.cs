namespace InvestmentTracker.Api.Contracts.Auth;

public sealed record LoginUserSuccessResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string TokenType = "Bearer");
