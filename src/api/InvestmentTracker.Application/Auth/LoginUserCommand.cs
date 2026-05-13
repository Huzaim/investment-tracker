using MediatR;

namespace InvestmentTracker.Application.Auth;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResult>;
