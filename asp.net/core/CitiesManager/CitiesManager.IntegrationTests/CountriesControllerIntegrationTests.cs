using CitiesManager.Core.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace CitiesManager.IntegrationTests;

public class CountriesControllerIntegrationTests(CustomWebApplicationFactory factory) 
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetCitiesByCountry_WhenCountryDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var countryId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/countries/{countryId}/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Detail.Should().Be("Country Not Found");
    }

    [Fact]
    public async Task GetCitiesByCountry_WhenCountryExists_ShouldReturnCitiesAnd200OK()
    {
        // Arrange
        var countryId = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-E1F2A3B4C5D6");

        // Act
        var response = await _client.GetAsync($"/api/countries/{countryId}/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cities = await response.Content.ReadFromJsonAsync<List<CityResponse>>();
        cities.Should().NotBeNull();
        cities.Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostCountry_WithInvalidData_ShouldReturn400BadRequestAndValidationErrors()
    {
        // Arrange
        var countryName = new CountryAddRequest("E");

        // Act
        var response = await _client.PostAsJsonAsync("/api/countries", countryName);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Errors.Should().ContainKey("CountryName");
    }


    [Fact]
    public async Task PostCountry_WithValidData_ShouldReturn201CreatedAndCountryResponse()
    {
        // Arrange
        var countryName = $"Spain_{Guid.NewGuid().ToString()[..4]}";
        var request = new CountryAddRequest(countryName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/countries", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var country = await response.Content.ReadFromJsonAsync<CountryResponse>();
        country.Should().NotBeNull();
        country.Id.Should().NotBe(Guid.Empty);
        country.CountryName.Should().Be(countryName);
    }

}
