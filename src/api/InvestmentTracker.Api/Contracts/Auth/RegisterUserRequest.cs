using System.ComponentModel.DataAnnotations;

namespace InvestmentTracker.Api.Contracts.Auth;

public sealed record RegisterUserRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(8)] string Password);
