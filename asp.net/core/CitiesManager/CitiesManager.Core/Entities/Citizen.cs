namespace CitiesManager.Core.Entities;

// Entity that represents a person who lives in a specific city
public class Citizen
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;

    // Foreign Key for City (Many-to-One relationship)
    public Guid CityId { get; set; }
    public virtual City? City { get; set; }
}