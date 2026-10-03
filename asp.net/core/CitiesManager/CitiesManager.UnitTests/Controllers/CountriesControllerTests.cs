using CitiesManager.Core.DTO;
using CitiesManager.Core.Entities;
using CitiesManager.Infrastructure.DatabaseContext;
using CitiesManager.WebAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.NSubstitute;
using NSubstitute;

namespace CitiesManager.UnitTests.Controllers;

public class CountriesControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly CountriesController _controller;

    public CountriesControllerTests()
    {
        _context = Substitute.For<ApplicationDbContext>();

        _controller = new CountriesController(_context);
    }

    #region GetCountry

    [Fact]
    public async Task GetCountry_ShouldReturnCountry_WhenCountryExists()
    {
        // Arrange
        var country = CreateCountry();

        var countries = new List<Country> { country };
        var countriesMock = countries.BuildMockDbSet();
        
        _context.Countries = countriesMock;

        // Act
        var result = await _controller.GetCountry(country.Id);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<CountryResponse>();
        result.Value.Id.Should().Be(country.Id);
        result.Value.CountryName.Should().Be(country.Name);
    }

    [Fact]
    public async Task GetCountry_ShouldReturnBadRequest_WhenCountryDoesNotExist()
    {
        // Arrange
        var countryId = Guid.NewGuid();
        var countriesMock = new List<Country>().BuildMockDbSet();

        _context.Countries = countriesMock;

        // Act
        var result = await _controller.GetCountry(countryId);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>(); // Problem() returns an ObjectResult
        result.Result.As<ObjectResult>().StatusCode.Should().Be(400);
    }

    #endregion

    #region Helpers

    private static Country CreateCountry()
    {
        return new Country
        {
            Id = Guid.NewGuid(),
            Name = "Spain",
            Cities = []
        };
    }

    #endregion
}
