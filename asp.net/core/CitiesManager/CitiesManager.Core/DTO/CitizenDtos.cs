using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO;

public record CitizenAddRequest(
    [Required]
    [StringLength(150, MinimumLength = 3)]
    string FullName,

    [Required]
    [DataType(DataType.Date)]
    DateTime DateOfBirth,

    [Required]
    [StringLength(200)]
    string Address
);

public record CitizenUpdateRequest(
    [Required]
    Guid CitizenId,

    [Required]
    [StringLength(150, MinimumLength = 3)]
    string FullName,

    [Required]
    [DataType(DataType.Date)]
    DateTime DateOfBirth,

    [Required]
    [StringLength(200)]
    string Address,

    [Required]
    Guid CityId
);
public record CitizenResponse(
    Guid CitizenId,
    string FullName,
    DateTime DateOfBirth,
    string Address,
    
    Guid CityId,
    string? CityName 
);