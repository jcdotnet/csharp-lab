using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO;

public record CountryAddRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string CountryName
);

public record CountryResponse(
    Guid Id,
    string CountryName,

    ICollection<string> CityNames
);
