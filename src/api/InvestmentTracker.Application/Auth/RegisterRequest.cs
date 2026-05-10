using System.ComponentModel.DataAnnotations;

namespace InvestmentTracker.Application.Auth;

public sealed record RegisterRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, MinLength(8)] string Password);
