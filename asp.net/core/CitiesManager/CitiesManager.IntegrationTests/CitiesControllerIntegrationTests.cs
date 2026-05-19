using CitiesManager.Core.DTO;
using CitiesManager.Core.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;

namespace CitiesManager.IntegrationTests;

public class CitiesControllerIntegrationTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetCities_ShouldReturn200OKAndListOfCities()
    {
        // Arrange
        // Act
        var response = await _client.GetAsync("/api/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var cities = await response.Content.ReadFromJsonAsync<List<CityResponse>>();

        cities.Should().NotBeNull();
        if (cities.Count > 0)
        {
            var city = cities.First();
            city.CityId.Should().NotBe(Guid.Empty);
            city.Name.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async Task GetCity_WhenCityDoesNotExist_ShouldReturn400BadRequestAndProblemDetails()
    {
        // Arrange
        var cityId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/cities/{cityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDeatils = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDeatils.Should().NotBeNull();
        problemDeatils.Title.Should().Be("Get City");
        problemDeatils.Detail.Should().Be("Invalid City Id");
    }

    [Fact]
    public async Task PostCity_WhenCountryDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var countryId = Guid.NewGuid();
        var request = new CityAddRequest("Barcelona", countryId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cities", request);

        // Assert
        // Maybe TODO later: The fake CountryId triggers a database FOREIGN KEY error (in the logs)
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task PostCity_WhenCityNameAlreadyExists_ShouldReturn409Conflict()
    {
        // Arrange
        var country = await GetCountryAsync();
        var cityName = $"DuplicateCity_{Guid.NewGuid().ToString()[..4]}";

        await _client.PostAsJsonAsync("/api/cities", new CityAddRequest(cityName, country!.Id));

        // Act
        var response = await _client.PostAsJsonAsync("/api/cities", new CityAddRequest(cityName, country.Id));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Post City");
        problemDetails.Detail.Should().Be("City Exists");
    }

    [Fact]
    public async Task PostCity_WithValidData_ShouldReturn201CreatedAndCityResponse()
    {
        // Arrange
        var country = await GetCountryAsync();
     
        var cityName = $"Málaga_{Guid.NewGuid().ToString()[..4]}";
        var cityRequest = new CityAddRequest(cityName, country!.Id);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cities", cityRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var city = await response.Content.ReadFromJsonAsync<CityResponse>();
        city.Should().NotBeNull();
        city.CityId.Should().NotBe(Guid.Empty);
        city.Name.Should().Be(cityName);
    }

    // TODO: maybe consider removing the ID property from the DTO (same as I did in CityAddRequest)
    [Fact]
    public async Task PutCity_WhenIdsDoNotMatch_ShouldReturn400BadRequest()
    {
        // Arrange
        var cityIdFromUrl = Guid.NewGuid();
        var cityIdFromBody = Guid.NewGuid();
        var countryId = Guid.NewGuid();

        var request = new CityUpdateRequest(cityIdFromBody, "Sevilla Actualizada", countryId);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/cities/{cityIdFromUrl}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PutCity_WWhenCityNameAlreadyExists_ShouldReturn409Conflict()
    {
        // Arrange
        var country = await GetCountryAsync();
        var city = await GetCityAsync(country);
        var existingCity = await GetCityAsync(country);

        // Update the city using using the existing city name
        var updateRequest = new CityUpdateRequest(city!.CityId, existingCity!.Name, country!.Id);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/cities/{city.CityId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Put City");
        problemDetails.Detail.Should().Be("City Exists");
    }

    [Fact]
    public async Task GetCitizens_WhenCityExists_ShouldReturn200OKAndListOfCitizens()
    {
        // Arrange
        var country = await GetCountryAsync();
        var city = await GetCityAsync(country);

        // Act
        var response = await _client.GetAsync($"/api/cities/{city!.CityId}/Citizens");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var citizens = await response.Content.ReadFromJsonAsync<List<CitizenResponse>>();

        citizens.Should().NotBeNull();
        if (citizens!.Count > 0)
        {
            var firstCitizen = citizens.First();
            firstCitizen.CitizenId.Should().NotBe(Guid.Empty);
            firstCitizen.FullName.Should().NotBeNullOrWhiteSpace();
        }
    }


    [Fact]
    public async Task PostCitizen_WhenCityDoesNotExist_ShouldReturn404NotFound()
    {
        // Arrange
        var cityId = Guid.NewGuid();
        var request = new
        {
            FullName = "Antonio Banderas",
            DateOfBirth = "1960-08-10",
            Address = "Centro histórico"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/cities/{cityId}/citizens", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails.Title.Should().Be("Register Citizen");
        problemDetails.Detail.Should().Be("City Not Found");
    }

    [Fact]
    public async Task PostCitizen_WithValidData_ShouldReturn201CreatedAndCitizenResponse()
    {
        // Arrange
        var country = await GetCountryAsync();
        var city = await GetCityAsync(country);

        var citizenName = $"Antonio_Banderas{Guid.NewGuid().ToString()[..4]}";
        var request = new
        {
            FullName = citizenName,
            DateOfBirth = "1960-08-10",
            Address = "Centro histórico"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/cities/{city!.CityId}/citizens", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var citizen = await response.Content.ReadFromJsonAsync<CitizenResponse>();
        citizen.Should().NotBeNull();
        citizen!.FullName.Should().Be(citizenName);
    }

    private async Task<CountryResponse?> GetCountryAsync()
    {
        var countryName = $"Spain_{Guid.NewGuid().ToString()[..4]}";
        var countryResponse = await _client.PostAsJsonAsync("/api/countries", new CountryAddRequest(countryName));
        return await countryResponse.Content.ReadFromJsonAsync<CountryResponse>();
    }

    private async Task<CityResponse?> GetCityAsync(CountryResponse? country)
    {
        var cityName = $"Málaga_{Guid.NewGuid().ToString()[..4]}";
        var cityResponse = await _client.PostAsJsonAsync("/api/cities", new CityAddRequest(cityName, country!.Id));
        return await cityResponse.Content.ReadFromJsonAsync<CityResponse>();
    }
    
}
