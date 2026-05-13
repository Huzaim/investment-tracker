using MediatR;

namespace InvestmentTracker.Application.Auth;

public sealed record RegisterUserCommand(
    string Email,
    string Password) : IRequest<RegisterUserResult>;
