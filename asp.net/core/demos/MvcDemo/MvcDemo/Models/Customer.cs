namespace MvcDemo.Models
{
    public class Customer
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }

    }

    public enum Gender
    {
        Male, Female, Other

    }
}
