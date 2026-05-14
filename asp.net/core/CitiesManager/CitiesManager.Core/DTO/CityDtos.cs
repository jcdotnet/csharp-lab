using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO;

public record CityAddRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string CityName,

    [Required]
    Guid CountryId
);

public record CityUpdateRequest(
    [Required]
    Guid CityId,

    [Required]
    [StringLength(100, MinimumLength = 2)]
    string CityName,

    [Required]
    Guid CountryId
);

public record CityResponse(
    Guid CityId,
    string CityName,
    Guid CountryId,

    string? CountryName,
    int? Population 
);
