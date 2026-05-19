using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO;

public record CityAddRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required]
    Guid CountryId
);

public record CityUpdateRequest(
    [Required]
    Guid CityId,

    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required]
    Guid CountryId
);

public record CityResponse(
    Guid CityId,
    string Name,
    Guid CountryId,

    string? CountryName,
    int? Population 
);
