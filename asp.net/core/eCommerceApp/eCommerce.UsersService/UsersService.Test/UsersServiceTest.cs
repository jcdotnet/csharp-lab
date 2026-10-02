using AutoMapper;
using eCommerce.Application.DTO;
using eCommerce.Application.RepositoryContracts;
using eCommerce.Domain.Entities;
using FluentAssertions;
using Moq;

using UsersServiceClass = eCommerce.Application.Services.UsersService;

namespace UsersService.Test;

public class UsersServiceTest
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly UsersServiceClass _service;

    public UsersServiceTest()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new UsersServiceClass(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    #region GetUser

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var applicationUser = new ApplicationUser { UserId = userId };
        var userDto = new UserDto(userId, "john@example.com", "John", "Male");

        _usersRepositoryMock.Setup(r => r.GetUserByUserId(userId)).ReturnsAsync(applicationUser);
        _mapperMock.Setup(m => m.Map<UserDto>(applicationUser)).Returns(userDto);

        // Act
        var user = await _service.GetUser(userId);

        // Assert
        user.Should().Be(userDto);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNull_WhenUserDoesNotExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        ApplicationUser? applicationUser = null;

        _usersRepositoryMock.Setup(r => r.GetUserByUserId(userId)).ReturnsAsync(applicationUser);

        // Act
        var user = await _service.GetUser(userId);

        // Assert
        user.Should().BeNull();
    }

    #endregion

    #region Login

    [Fact]
    public async Task Login_ShouldReturnAuthenticationResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var loginRequest = new LoginRequest("john@example.com", "password123");
        var applicationUser = new ApplicationUser
        {
            UserId = Guid.NewGuid(),
            Email = loginRequest.Email,
            UserName = "John",
            Gender = "Male"
        };
        var authenticationResponse = new AuthenticationResponse(
            applicationUser.UserId,
            applicationUser.Email,
            applicationUser.UserName,
            applicationUser.Gender,
            Token: "token",
            Success: true
        );

        _usersRepositoryMock.Setup(r => r.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(applicationUser);
        _mapperMock.Setup(m => m.Map<AuthenticationResponse>(applicationUser)).Returns(authenticationResponse);

        // Act
        var result = await _service.Login(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(authenticationResponse);
    }

    [Fact]
    public async Task Login_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginRequest = new LoginRequest("john@example.com", "invalidPassword");
        ApplicationUser? applicationUser = null;

        _usersRepositoryMock.Setup(r => r.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password))
            .ReturnsAsync(applicationUser);

        // Act
        var result = await _service.Login(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Register

    [Fact]
    public async Task Register_ShouldReturnAuthenticationResponse_WhenSuccessful()
    {
        // Arrange
        var registerRequest = new RegisterRequest("john@example.com", "password123", "John", GenderOptions.Male);
        var applicationUser = new ApplicationUser
        {
            UserId = Guid.NewGuid(),
            UserName = registerRequest.UserName,
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            Gender = registerRequest.Gender.ToString()
        };
        var authenticationResponse = new AuthenticationResponse(
            applicationUser.UserId,
            applicationUser.Email,
            applicationUser.UserName,
            applicationUser.Gender,
            Token: "token",
            Success: true
        );

        _mapperMock.Setup(m => m.Map<ApplicationUser>(registerRequest)).Returns(applicationUser);
        _usersRepositoryMock.Setup(r => r.AddUser(applicationUser)).ReturnsAsync(applicationUser);
        _mapperMock.Setup(m => m.Map<AuthenticationResponse>(applicationUser)).Returns(authenticationResponse);

        // Act
        var result = await _service.Register(registerRequest);

        // Assert
        result.Should().Be(authenticationResponse);
    }

    [Fact]
    public async Task Register_ShouldReturnNull_WhenUserNotRegistered()
    {
        // Arrange
        var registerRequest = new RegisterRequest("john@example.com", "password123", "John", GenderOptions.Male);
        var applicationUser = new ApplicationUser
        {
            UserId = Guid.NewGuid(),
            UserName = registerRequest.UserName,
            Email = registerRequest.Email,
            Password = registerRequest.Password,
            Gender = registerRequest.Gender.ToString()
        };
    
        _mapperMock.Setup(m => m.Map<ApplicationUser>(registerRequest)).Returns(applicationUser);
        _usersRepositoryMock.Setup(r => r.AddUser(applicationUser)).ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await _service.Register(registerRequest);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
