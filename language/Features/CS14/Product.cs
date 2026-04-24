namespace CS14
{
    // Class to test field backed properties
    public class Product(Guid productId)
    {
        public Guid ProductId { get; set; } =  productId;

        public string ProductName
        {
            // The keyword 'field' replaces the need for a manual private backing field
            get;
            set => field = string.IsNullOrWhiteSpace(value) ? "Unnamed Product" : value.Trim();
        } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public double? UnitPrice
        {
            get;
            set => field = value < 0 ? 0 : value;
        }

        public int? QuantityInStock
        {
            get;
            set => field = value ?? 0;
        }
    }
}
