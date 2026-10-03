using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CitiesManager.UnitTests.Services;

public class JwtServiceTests
{
    private readonly JwtService _service;
    public JwtServiceTests()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "this-is-a-test-key-with-enough-length-123456",
            ["Jwt:Issuer"] = "TestIssuer",
            ["Jwt:Audience"] = "TestAudience",
            ["Jwt:ExpirationMinutes"] = "30",
            ["RefreshToken:ExpirationMinutes"] = "60"
        }).Build();

        _service = new JwtService(configuration);
    }

    [Fact]
    public void CreateJwtToken_ShouldReturnAuthenticationResponse_WhenUserIsValid()
    {
        // Arrange
        var applicationUser = CreateApplicationUser();

        // Act
        var result = _service.CreateJwtToken(applicationUser);

        // Assert
        result.Should().BeOfType<AuthenticationResponse>();
        result.Name.Should().Be(applicationUser.Name);
        result.Email.Should().Be(applicationUser.Email);

        // Token
        result.Token.Should().NotBeNullOrWhiteSpace();
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.Expiration.Should().BeAfter(DateTime.UtcNow);
        result.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void GetPrincipalFromJwtToken_ShouldReturnPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var applicationUser = CreateApplicationUser();
        var authenticationResponse = _service.CreateJwtToken(applicationUser);

        // Act
        var result = _service.GetPrincipalFromJwtToken(authenticationResponse.Token);

        // Assert
        result.Should().NotBeNull();

        // User ID, Name and Email are included in the JWT claims
        result.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(applicationUser.Id.ToString());
        result.FindFirst(ClaimTypes.Name)!.Value.Should().Be(applicationUser.Name);
        result.FindFirst(ClaimTypes.Email)!.Value.Should().Be(applicationUser.Email);
    }

    [Fact]
    public void GetPrincipalFromJwtToken_ShouldThrowException_WhenTokenIsInvalid()
    {
        // Arrange
        var token = "invalid-token";

        // Act
        var action = () => _service.GetPrincipalFromJwtToken(token);

        // Assert
        action.Should().Throw<Exception>();
    }


    #region Helpers

    private static ApplicationUser CreateApplicationUser()
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john@example.com"
        };
    }

    #endregion
}
