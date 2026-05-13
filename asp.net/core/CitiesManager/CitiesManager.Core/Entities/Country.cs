namespace CitiesManager.Core.Entities;

public class Country
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<City> Cities { get; set; } = [];
}
