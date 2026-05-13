namespace CitiesManager.Core.Entities;

public class CityDetail
{
    // Shared Primary Key for clean 1-to-1 relationship
    public Guid CityId { get; set; }

    public long Population { get; set; }
    public int AverageSunnyDays { get; set; }
    public string MayorName { get; set; } = string.Empty;

    // Navigation property
    public virtual City? City { get; set; }
}
