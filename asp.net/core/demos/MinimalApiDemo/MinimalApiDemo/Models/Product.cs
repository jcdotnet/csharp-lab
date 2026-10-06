using System.ComponentModel.DataAnnotations;

namespace MinimalApiDemo.Models
{
    public class Product
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public override string ToString()
        {
            return $"{Id}, {Name}";
        }
    }
}
