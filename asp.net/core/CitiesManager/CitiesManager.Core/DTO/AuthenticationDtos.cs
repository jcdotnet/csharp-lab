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

public record RegisterDto(
    [Required]
    string Name,

    [Required]
    [EmailAddress]
    [property: Remote(action: "ValidateEmail", controller: "Account", ErrorMessage = "Email exists")] 
    string Email,

    [Required]
    [RegularExpression("^[0-9]{3,12}$", ErrorMessage = "Phone number must contain 3-12 digits")]
    string PhoneNumber,

    [Required]
    string Password,

    [Required]
    [property: Compare("Password", ErrorMessage = "Passwords don't match")]
    string ConfirmPassword
);
public record TokenDto(
    string? Token,
    string? RefreshToken
);