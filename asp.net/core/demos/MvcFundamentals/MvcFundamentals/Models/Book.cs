namespace Demo.Models
{
    public class Book // POCO (= Plain Old CLR Object)
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }

        public override string? ToString()
        {
            return $"{Title}, {Author}";
        }
    }
}
