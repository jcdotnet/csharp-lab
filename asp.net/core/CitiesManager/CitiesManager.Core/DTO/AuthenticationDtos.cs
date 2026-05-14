using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO;

public record AuthenticationResponse(
    string Name,
    string Email,
    string Token,
    string RefreshToken,
    DateTime Expiration,
    DateTime RefreshTokenExpiration
);

public record LoginDto(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Password
);

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Remote(action: "ValidateEmail", controller: "Account", ErrorMessage = "Email exists")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[0-9]{3,12}$", ErrorMessage = "Phone number must contain 3-12 digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public record TokenDto(
    string? Token,
    string? RefreshToken
);