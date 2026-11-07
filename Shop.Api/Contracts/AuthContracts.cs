using System.ComponentModel.DataAnnotations;

namespace Shop.Api.Contracts;

public sealed record LoginRequest(
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(256)] string Password);

public sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresIn);
