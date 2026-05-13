using System.ComponentModel.DataAnnotations;

namespace InvestmentTracker.Api.Contracts.Auth;

public sealed record LoginUserRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password);
