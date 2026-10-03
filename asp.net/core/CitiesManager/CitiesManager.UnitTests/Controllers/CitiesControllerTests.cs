using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using CitiesManager.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CitiesManager.UnitTests.Controllers;

public class CitiesControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly CitiesController _controller;

    public CitiesControllerTests()
    {
        _context = Substitute.For<ApplicationDbContext>();

        _controller = new CitiesController(_context);
    }

    #region GetCity

    [Fact]
    public async Task GetCity_ShouldReturnCity_WhenCityExists()
    {
        // Arrange
        var city = CreateCity();
        var cities = new List<City> { city };
        var citiesMock= cities.BuildMockDbSet();

        _context.Cities = citiesMock;
        
        // Act
        var result = await _controller.GetCity(city.Id);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value!.CityId.Should().Be(city.Id);
        result.Value.Name.Should().Be(city.Name);
    }

    [Fact]
    public async Task GetCity_ShouldReturnBadRequest_WhenCityDoesNotExist()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var citiesMock = new List<City>().BuildMockDbSet();

        _context.Cities = citiesMock;

        // Act
        var result = await _controller.GetCity(cityId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>(); // Problem() returns an ObjectResult
        result.Result.As<ObjectResult>().StatusCode.Should().Be(400);
    }

    #endregion

    #region Helpers

    private static City CreateCity()
    {
        return new City
        {
            Id = Guid.NewGuid(),
            Name = "Málaga"
        };
    }

    #endregion
}
