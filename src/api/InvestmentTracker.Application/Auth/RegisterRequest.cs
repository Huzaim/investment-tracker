using System.ComponentModel.DataAnnotations;

namespace InvestmentTracker.Application.Auth;

public sealed record RegisterRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(8)] string Password);
