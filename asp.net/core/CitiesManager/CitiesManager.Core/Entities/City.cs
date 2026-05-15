namespace CitiesManager.Core.Entities;

// Entity model for City -  Clean POCO architecture
public class City
{
    //[Key] // DB configuration moved to FluentAPI (SRP)
    public Guid Id { get; set; }

    //[Required] // DB configuration moved to FluentAPI (SRP)
    public string Name { get; set; } = string.Empty;

    // Foreign Key for Country (Many-to-One relationship)
    public Guid CountryId { get; set; }
    public virtual Country? Country { get; set; }

    // Shared Primary Key (One-to-One relationship)
    public virtual CityDetail? Detail { get; set; }

    // One-to-Many relationship
    public virtual ICollection<Citizen> Citizens { get; set; } = [];
}
