using System.ComponentModel.DataAnnotations;
using MediatR;

namespace InvestmentTracker.Application.Auth;

public sealed record RegisterUserCommand(
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(8)] string Password) : IRequest<RegisterUserResult>;
